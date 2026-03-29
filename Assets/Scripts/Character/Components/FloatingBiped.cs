using System;
using System.Collections;
using UnityEngine;

class HitInformation
{
    public bool hit;

    public Vector3 hitLocation;

    public Vector3 hitNormal;

    public float hitDistance;

    public GameObject hitObject;

    public Rigidbody hitRigid;

    public HitInformation(bool hit, Vector3 hitLocation, Vector3 hitNormal, float hitDistance, GameObject hitObject, Rigidbody rigid)
    {
        this.hit = hit;
        this.hitLocation = hitLocation;
        this.hitNormal = hitNormal;
        this.hitDistance = hitDistance;
        this.hitObject = hitObject;
        this.hitRigid = rigid;
    }

    public HitInformation(bool hit)
    {
        hit = false;
    }
}


[RequireComponent(typeof(Rigidbody))]
public class FloatingBiped : MovementProvider
{

    private Rigidbody rig;
    
    [Header("Character Movement Variables")]
    [SerializeField] private float playerAcceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float airMovementInfluence = 0.2f;
    [SerializeField] private float maxAccelerationForce;
    [SerializeField] private float decelerationCoefficient = 0.3f;
    [SerializeField] private float frictionCoefficient = 0.98f;
    [SerializeField] private float groundHugForce = 20;
    [SerializeField] private AnimationCurve accelerationFromDot;
    [SerializeField] private AnimationCurve maxAccelerationFromDot;
    [SerializeField] private AnimationCurve playerAccelerationCurve;
    
    [Header("Jumping Properties")] 
    [SerializeField] private bool canJump = false;

    [SerializeField] private float jumpGroundNormalInfluence = 0.5f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpInterval = 0.2f;
    [SerializeField] private float maxJumpTime = 0.3f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpLength;
    [SerializeField] private float terminalJumpVelocity; //Max Jumping velocity
    
    private bool isJumping = false;
    private float jumpTime;
    
    float lastGroundedTime;

    [Header("Player Spring Settings")] 
    [SerializeField] private float rideOffset = 0;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideDampnerForce;
    
    [SerializeField] private AnimationCurve slopeGripFactor;
    
    // Smoothly apply resistance between these angles                
    float resistStartAngle = 20f; // begin resisting                 
    float resistFullAngle  = 55; // completely block uphill motion  
    float uphillResistance = 1.3f;  // scaling factor    
    
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool canStand = false;
    
    Vector3 desiredDir;

    Vector3 m_UnitGoal; //targetDir?

    Vector3 m_GoalVel; //target Velocity
    
    private Rigidbody hitObject=null;

    private void Start()
    {
        rig=GetComponent<Rigidbody>();
        
        desiredDir = transform.forward;

        jumpTime = Time.time;
        lastGroundedTime = 999999;
        
        if (slopeGripFactor.length > 0)
        {
            resistStartAngle = slopeGripFactor.keys[0].value;

            resistFullAngle = slopeGripFactor.keys[slopeGripFactor.length - 1].value;
        }
        
    }
    
    private float GetGroundAngleRelativeToGravity()
    {
        RaycastHit hit;
        float slopeAngle = 0;
        if(Physics.Raycast(transform.position, transform.up * -1, out hit, rideHeight * 1.2f,~0, QueryTriggerInteraction.Ignore))
        {
            slopeAngle = Vector3.Angle(hit.normal, -Physics.gravity);
        }
        return slopeAngle;
    }
    
    private HitInformation RaycastFromBody(Transform centre)
    {
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("PlayerTests");
        if (Physics.Raycast(centre.transform.position- new Vector3(0,rideOffset,0), centre.transform.up * -1, out hit, rideHeight, layerMask))
        {
            return new HitInformation(true, hit.point, hit.normal, hit.distance, hit.collider.gameObject,hit.rigidbody);
        }
        return new HitInformation(false);
    }
    
    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
    Quaternion ShortestRotation(Quaternion target, Quaternion current)
    {
        if (Quaternion.Dot(target, current) < 0)
        {
            return target * Quaternion.Inverse(Multiply(current, -1));
        }
        else return target * Quaternion.Inverse(current);
    }

    private Vector3 groundVelocity = Vector3.zero;
    
    private Vector3 groundNormal = Vector3.zero;
    
    private float gripRatio;
    
    private void ManageSpring()
    {
        HitInformation hitInfo = RaycastFromBody(transform);

        groundNormal = hitInfo.hitNormal;
        
        //Lets think about different Gravity Directions later :)
        Vector3 downDir = Vector3.down;

         canStand = GetGroundAngleRelativeToGravity() < 40;

         gripRatio = slopeGripFactor.Evaluate(GetGroundAngleRelativeToGravity());
         
        //Manage Coyote Time
        if (hitInfo.hit == false)
        {
            if (Time.time - lastGroundedTime > coyoteTime || isJumping)
            {
                canJump = false;
            }
        }
        else
        {
            if (Time.time - jumpTime > jumpInterval)
            {
                canJump = true;
            }
        }
        

        //Jump checks later
        isGrounded = hitInfo.hit;
        
        bool jumpGracePeriod = Time.time - jumpTime < 0.15f;

        if (canStand == false)
        {
            return;
        }
        
        if (hitInfo.hit && !jumpGracePeriod)
        {
            jumpNormal = Vector3.Slerp(Vector3.up, groundNormal, jumpGroundNormalInfluence).normalized;
            //Used to calculate if the user can jump

            if (!isJumping)
            {
                lastGroundedTime = Time.time;
            }

            //Velocity comparisons for managing spring force
            Vector3 velocity= rig.linearVelocity;

            Vector3 rayDir= transform.TransformDirection(downDir);

            Vector3 otherVel = Vector3.zero;
            
            
            
            hitObject = hitInfo.hitRigid;


            if(hitObject != null) //Store relative velocity of platforms for later use
            {
                groundVelocity = hitObject.GetPointVelocity(hitInfo.hitLocation);
                otherVel = groundVelocity;
            }
            else
            {
                groundVelocity = Vector3.zero;
            }

            //store dot product of velocities compared to player upright direction
            float rayDirVel=Vector3.Dot(rayDir,velocity);

            float otherDirVel= Vector3.Dot(rayDir,otherVel);
            
            
            //store results to work out spring strength required to float the player
            float relVel = rayDirVel - otherDirVel;

            float x = hitInfo.hitDistance -rideHeight;

            float springForce = (x*rideSpringStrength) - (relVel*rideDampnerForce);

            //Float player
            if (!isJumping && !jumpPressed && (Time.time - jumpTime > 0.1f))
            {
                rig.AddForce(springForce*gripRatio*rayDir);

                if (hitObject != null) //Add opposite spring force to object to simulate standing on it
                {
                    hitObject.AddForceAtPosition(rayDir * -springForce, hitInfo.hitLocation);
                }
            }
        }
        else
        {
            groundVelocity=Vector3.zero;
        }
        
        
    }
    
    public float jumpSlowDown = 0.1f;
    IEnumerator JumpSlowDown()
    {
        float targetVelocity = 0;
        float velocityY = rig.linearVelocity.y;

        float t = 0;

        while (velocityY > 1f)
        {
            velocityY=rig.linearVelocity.y;
            velocityY = Mathf.Lerp(velocityY, targetVelocity, t);
            t += jumpSlowDown*Time.fixedDeltaTime;
            
            rig.linearVelocity = new Vector3(rig.linearVelocity.x, velocityY, rig.linearVelocity.z);
            yield return new WaitForFixedUpdate();
        }



        yield return null;
    }

    private void CalculateJumpForce()
    {
        if (isJumping)
        {
            rig.AddForce(jumpForce*(1-(Time.time-jumpTime)/jumpLength)*jumpNormal);
            
        }
    }

    private Vector3 jumpNormal = Vector3.up;
    bool jumpPressed = false;
    void ManageJump()
    {
        if (isJumping)
        {
            if (Time.time - jumpTime > jumpLength || !jumpPressed)
            {
                if (!jumpPressed & isJumping)
                {
                    StartCoroutine(JumpSlowDown());
                }
                isJumping = false;


                //StartCoroutine(JumpSlowDown());
            }

        }
        else
        {
            if (jumpPressed)
            {
                if (canJump)
                {
                    jumpTime = Time.time;
                    isJumping=true;
                    rig.linearVelocity = new Vector3(rig.linearVelocity.x, 0, rig.linearVelocity.z);
                    rig.AddForce(jumpNormal * jumpForce*1.3f, ForceMode.VelocityChange);

                    if (hitObject != null)
                    {
                        hitObject.AddForce(-jumpNormal * jumpForce*1.3f, ForceMode.VelocityChange);
                    }
                }
            }
        }
        
        CalculateJumpForce();

      
    }
    
    void ManageFriction()
    {
        float yVelocity = rig.linearVelocity.y;
        // generic Drag
        if (!isGrounded)
        {
            
            rig.linearVelocity *= 0.99f;
            
            rig.linearVelocity= new Vector3(rig.linearVelocity.x,yVelocity,rig.linearVelocity.z);
            
            return;
        }

        
        Vector3 relativeVel = rig.linearVelocity - groundVelocity;

        yVelocity = rig.linearVelocity.y;
        
        // blend velocity towards ground velocity direction 
        rig.linearVelocity = Vector3.Lerp(rig.linearVelocity, groundVelocity, 0.1f);
    
        // if there is no player input add lots of friction;
        if (playerInputs.magnitude == 0)
        {
            relativeVel *= frictionCoefficient;
            rig.linearVelocity = groundVelocity + relativeVel;
        }
        
        rig.linearVelocity= new Vector3(rig.linearVelocity.x,yVelocity,rig.linearVelocity.z);
    }
    private void ManageMovement()
    {
    
        // 
        //Air Movement modifiers
        float airTimeModifier = 1;

        if (!isGrounded)
        {
            airTimeModifier = airMovementInfluence;
        }
        
        //Add ground velocity to player
        if (hitObject != null)
        {

            //fixed delta time before
            //rig.AddForce(hitObject.linearVelocity * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        Vector3 neededAccel = Vector3.zero;
        
        // 

        // Project movement dir along a plane so that we can travel over slopes more efficently
        Vector3 movementDir = Vector3.ProjectOnPlane(desiredDir, groundNormal).normalized;
        m_UnitGoal = movementDir;



        // Calculate velocity for directions
        Vector3 unitVel = rig.linearVelocity.sqrMagnitude > 0.3f ? rig.linearVelocity.normalized : rig.transform.forward*0.3f;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
                                  
        //Get potential acceleration change from current velocity
        float accelDot = Mathf.Max(accelerationFromDot.Evaluate(velDot), 0.4f);
        float maxAccelDot = Mathf.Max(maxAccelerationFromDot.Evaluate(velDot), 0.4f);

        float tempAcceleration = playerAcceleration;
        
        tempAcceleration*=Mathf.Clamp01(playerAccelerationCurve.Evaluate(rig.linearVelocity.magnitude/maxSpeed)*2);


        float accel = tempAcceleration * accelDot;
        
        
        accel *= airTimeModifier;
        
        float maxAccel = maxAccelerationForce * maxAccelDot;
        
        //Calculate goal velocity for our player
        float targetSpeed = maxSpeed * Mathf.Clamp01(playerInputs.magnitude);
        
        

        Vector3 goalVel = m_UnitGoal * targetSpeed;
        m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accel * Time.fixedDeltaTime);
        
        // 2. Handle Idle / No Input
        if (playerInputs.magnitude == 0)
        {
            // On ground, we slow down to a halt
            if (isGrounded)
            {
                m_GoalVel = Vector3.MoveTowards(m_GoalVel, Vector3.zero, playerAcceleration *decelerationCoefficient* Time.fixedDeltaTime);
                neededAccel = (m_GoalVel - rig.linearVelocity) / Time.fixedDeltaTime;
                
                neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelerationForce * decelerationCoefficient);
            }
        }
        else
        {
            //calculate desired acceleration and clamp that to the max acceleration
            neededAccel = (m_GoalVel - rig.linearVelocity) / Time.fixedDeltaTime;
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        }

        // [UPHILL MODIFIERS FOR PLAYER MOVEMENT]
        
        // Compute uphill direction (the direction up the slope surface)
        Vector3 uphillDir = Vector3.ProjectOnPlane(Vector3.up, groundNormal);
        if (uphillDir.sqrMagnitude > 0.0001f)
            uphillDir.Normalize();
        else
            uphillDir = Vector3.zero;

        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);






        if (slopeAngle > resistStartAngle && uphillDir != Vector3.zero)
        {
            // how much of the acceleration points uphill
            Vector3 uphillAccel = Vector3.Project(neededAccel, uphillDir);
            float uphillDot = Vector3.Dot(uphillAccel.normalized, uphillDir);

            if (uphillAccel.magnitude > 0f && uphillDot > 0f)
            {
                // Compute how steep the slope is (0–1)
                float steepnessRatio = Mathf.InverseLerp(resistStartAngle, resistFullAngle, slopeAngle);

                // Reduce uphill acceleration only
                uphillResistance = Mathf.Lerp(1f, 0f, steepnessRatio);


                Vector3 reducedUphill = uphillAccel * uphillResistance;

                reducedUphill *= gripRatio;
                
                Vector3 otherAccel = neededAccel - uphillAccel;

                neededAccel = otherAccel + reducedUphill;
                // Ensure total accel doesn't exceed allowed cap
                neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            }
        }
        // --- Apply force ---
        Vector3 forceScale = new Vector3(1, 0, 1);
        rig.AddForce(Vector3.Scale(neededAccel * rig.mass, forceScale));
    }

    void ManageGroundHugForce()
    {
        if (!isGrounded && !isJumping && Time.time-lastGroundedTime<0.2f)
        {
            rig.AddForce(Vector3.down*groundHugForce, ForceMode.Impulse);
        }
    }

    private Vector2 playerInputs;
    private void FixedUpdate()
    {
        ManageJump(); 
        ManageGroundHugForce();
        ManageSpring();
        ManageFriction();
        ManageMovement();
    }

    public override void LookAtVector(Vector3 lookDir)
    {
        Vector3 rotation= transform.rotation.eulerAngles;

        rotation.y = lookDir.y;
        transform.rotation = Quaternion.Euler(rotation);
    }

    public override void Move(Vector2 moveInput)
    {
        playerInputs = moveInput;
        
        Vector3 moveDirection = (transform.forward * playerInputs.y + transform.right * playerInputs.x).normalized;

        desiredDir = moveDirection;
    }

    public override void Jump(bool jumpPress)
    {
        jumpPressed= jumpPress;
    }

    public override float GetSpeed()
    {
        return rig.linearVelocity.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position - new Vector3(0, rideOffset, 0),0.03f);
    }
}