using System;
using System.Numerics;
using Sandbox;

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

public static class Vector3Extensions
{
    public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 delta = target - current;
        float magnitude = delta.Length;

        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }

        return current + delta / magnitude * maxDistanceDelta;
    }
}
public sealed class FloatingBiped : MovementProvider
{
	
	private Rigidbody rig;

	//Player Movement Variables
	[Header("Player Movement Variables")]
	[Property] public float playerAcceleration { get; set; }
	[Property] private float maxSpeed;
	[Property] private float airMovementInfluence = 0.2f;
    [Property] private float maxAccelerationForce;
    [Property] private float decelerationCoefficient = 0.3f;
    [Property] private float frictionCoefficient = 0.98f;
    [Property] private float groundHugForce = 20;

	[Property] private Curve accelerationFromDot;
    [Property] private Curve maxAccelerationFromDot;
    [Property] private Curve playerAccelerationCurve;


	[Header("Jumping Properties")] 
    [Property] private bool canJump {get;set;}=false;

    [Property] private float jumpGroundNormalInfluence = 0.5f;
    [Property] private float coyoteTime = 0.2f;
    [Property] private float jumpInterval = 0.2f;
    [Property] private float maxJumpTime = 0.3f;
    [Property] private float jumpForce;
    [Property] private float jumpLength;
    [Property] private float terminalJumpVelocity; //Max Jumping velocity

	//Player Spring Settings
	[Header("Player Spring Settings")]
	[Property] float rideOffset {get; set;} =0;
    [Property] private float rideHeight;
    [Property] private float rideSpringStrength;
    [Property] private float rideDampnerForce;
	[Property] private Curve slopeGripFactor;

	[Property] private GameObject raycastPoint;


	//Local Variables

	Rigidbody hitObject;


	//Movement Variables

	Vector3 m_UnitGoal;

	Vector3 m_GoalVel;
	Vector3 groundVelocity;

	Vector3 desiredDir=Vector3.Zero;


	//jumping variables
	private bool isJumping=false;

	private bool jumpPressed=false;

	private float jumpTime=0;

	private Vector3 jumpNormal;

	float gripRatio=1;

	//grounded variables
	private Vector3 groundNormal;

	[Property] public bool canStand=false;
	[Property] private bool isGrounded=false;

	private float lastGroundedTime=0;

	protected override void OnStart()
	{
		
		rig=GetComponent<Rigidbody>();

		if(rig!=null)
		{
		Log.Info("We have our rig");
		}

	}

	private HitInformation RaycastFromBody(float lengthMultiplier=1)
    {
        float raycastLength=rideHeight*lengthMultiplier;
		var trace= Scene.PhysicsWorld.Trace.Ray(raycastPoint.WorldPosition,raycastPoint.WorldPosition+WorldTransform.Down*raycastLength).WithoutTag("player").Run();
        if (trace.Hit && trace.Body.GameObject!=GameObject)
        {
            return new HitInformation(true, trace.HitPosition, trace.Normal, trace.Distance, 
			trace.Body.GameObject,trace.Body.GameObject.GetComponent<Rigidbody>());
        }
        return new HitInformation(false);
    }

	private float GetGroundAngleRelativeToGravity()
    {
        var trace= Scene.PhysicsWorld.Trace.Ray(raycastPoint.WorldPosition,raycastPoint.WorldPosition+WorldTransform.Down*rideHeight).Run();
        float slopeAngle=0;

		if (trace.Hit)
        {		
			slopeAngle=Vector3.GetAngle(trace.Normal,-Scene.PhysicsWorld.Gravity);
        }
        return slopeAngle;
    }

	[Property] float debugShow=0;


	Vector2 playerInputs;
	/// <summary>
	/// Apply a small friction force depending on grounded and air states to naturally ease the player to stop
	/// </summary>
	void ManageFriction()
    {
        float zVelocity = rig.Velocity.z;
        // generic Drag
        if (!isGrounded)
        {
            
            rig.Velocity *= 0.99f;
            
            rig.Velocity= new Vector3(rig.Velocity.x,rig.Velocity.y,zVelocity);
            
            return;
        }

        
        Vector3 relativeVel = rig.Velocity - groundVelocity;

        zVelocity = rig.Velocity.z;
        
        // blend velocity towards ground velocity direction 
        rig.Velocity = Vector3.Lerp(rig.Velocity, groundVelocity, 0.1f);
    
        // if there is no player input add lots of friction;
        if (playerInputs.Length == 0)
        {
            relativeVel *= frictionCoefficient;
            rig.Velocity = groundVelocity + relativeVel;
        }
        
        rig.Velocity= new Vector3(rig.Velocity.x,rig.Velocity.y,zVelocity);
    }


    float resistStartAngle;
    float resistFullAngle;

    float uphillResistance=1;
	/// <summary>
	/// Player is held up by a spring from the ground, strength will define how
	/// well they can step over objects e.t.c
	/// </summary>
	private void ManageSpring()
    {
        HitInformation hitInfo = RaycastFromBody();

        groundNormal = hitInfo.hitNormal;


		hitObject=null;
        
        //Lets think about different Gravity Directions later :)
        Vector3 downDir = Vector3.Down;

        canStand = GetGroundAngleRelativeToGravity() < 40;

		debugShow=GetGroundAngleRelativeToGravity();
        gripRatio = slopeGripFactor.Evaluate(GetGroundAngleRelativeToGravity());
         
        //Manage Coyote Time
        if (hitInfo.hit == false)
        {
            if (Time.Now - lastGroundedTime > coyoteTime || isJumping)
            {
                canJump = false;
            }
        }
        else
        {
            if (Time.Now - jumpTime > jumpInterval)
            {
                canJump = true;
            }
        }
        

        //Jump checks later
        isGrounded = hitInfo.hit;

		if(hitObject!=null)
		{
		Log.Info(hitObject.GameObject.Name);
		}
        
        bool jumpGracePeriod = Time.Now - jumpTime < 0.15f;

        if (canStand == false)
        {
            return;
        }
        
        if (hitInfo.hit && !jumpGracePeriod)
        {
            jumpNormal = Vector3.Slerp(Vector3.Up, groundNormal, jumpGroundNormalInfluence).Normal;
            //Used to calculate if the user can jump

            if (!isJumping)
            {
                lastGroundedTime = Time.Now;
            }

            //Velocity comparisons for managing spring force
            Vector3 velocity= rig.Velocity;

            Vector3 rayDir= downDir;

            Vector3 otherVel = Vector3.Zero;
            
            
            
            hitObject = hitInfo.hitRigid;


            if(hitObject != null) //Store relative velocity of platforms for later use
            {
                groundVelocity = hitObject.GetVelocityAtPoint(hitInfo.hitLocation);
                otherVel = groundVelocity;
            }
            else
            {
                groundVelocity = Vector3.Zero;
            }

            //store dot product of velocities compared to player upright direction
            float rayDirVel=Vector3.Dot(rayDir,velocity);

            float otherDirVel= Vector3.Dot(rayDir,otherVel);
            
            
            //store results to work out spring strength required to float the player
            float relVel = rayDirVel - otherDirVel;

            float x = hitInfo.hitDistance -rideHeight;

            float springForce = (x*rideSpringStrength) - (relVel*rideDampnerForce);

            //Float player
            if (!isJumping && !jumpPressed && (Time.Now - jumpTime > 0.1f))
            {
                rig.ApplyForce(springForce*gripRatio*rayDir);

                if (hitObject != null) //Add opposite spring force to object to simulate standing on it
                {
                   // hitObject.ApplyForceAt(rayDir * -springForce, hitInfo.hitLocation);
                }
            }
        }
        else
        {
            groundVelocity=Vector3.Zero;
        }
        
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

        Vector3 neededAccel = Vector3.Zero;
        
        // 

        // Project movement dir along a plane so that we can travel over slopes more efficently
        Vector3 movementDir = Vector3.VectorPlaneProject(desiredDir, groundNormal).Normal;
        m_UnitGoal = movementDir;



        // Calculate velocity for directions
        Vector3 unitVel = rig.Velocity.LengthSquared > 0.3f ? rig.Velocity.Normal : WorldTransform.Forward*0.3f;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
                                  
        //Get potential acceleration change from current velocity
        float accelDot = Math.Max(accelerationFromDot.Evaluate(velDot), 0.4f);
        float maxAccelDot = Math.Max(maxAccelerationFromDot.Evaluate(velDot), 0.4f);

        float tempAcceleration = playerAcceleration;

		//CHANGE FOR A CLAMP01 equivalent next
        tempAcceleration*=Math.Clamp(playerAccelerationCurve.Evaluate(rig.Velocity.Length/maxSpeed)*2,0,1);



        float accel = tempAcceleration * accelDot;
        
        

        
        accel *= airTimeModifier;
        
        float maxAccel = maxAccelerationForce * maxAccelDot;


        
        //Calculate goal velocity for our player
        float targetSpeed = maxSpeed * Math.Clamp(playerInputs.Length,0,1);
        

        
        

        Vector3 goalVel = m_UnitGoal * targetSpeed;

        m_GoalVel = m_GoalVel.MoveTowards(goalVel, accel * Time.Delta);

        // 2. Handle Idle / No Input
        if (playerInputs.Length == 0)
        {
            // On ground, we slow down to a halt
            if (isGrounded)
            {
                m_GoalVel = m_GoalVel.MoveTowards(Vector3.Zero, playerAcceleration *decelerationCoefficient* Time.Delta);
                m_GoalVel= Vector3.Lerp(m_GoalVel,Vector3.Zero,playerAcceleration *decelerationCoefficient* Time.Delta*0.2f);
                neededAccel = (m_GoalVel - rig.Velocity) / Time.Delta;
                neededAccel = neededAccel.ClampLength(maxAccelerationForce * decelerationCoefficient);
                //neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelerationForce * decelerationCoefficient);
            }
        }
        else
        {
            //calculate desired acceleration and clamp that to the max acceleration
            neededAccel = (m_GoalVel - rig.Velocity) / Time.Delta;
            neededAccel = neededAccel.ClampLength(maxAccel);
        }

        // [UPHILL MODIFIERS FOR PLAYER MOVEMENT]
        
        // Compute uphill direction (the direction up the slope surface)
        Vector3 uphillDir = Vector3.VectorPlaneProject(Vector3.Up, groundNormal);
        if (uphillDir.LengthSquared > 0.0001f)
            uphillDir=uphillDir.Normal;
        else
            uphillDir = Vector3.Zero;

        float slopeAngle = Vector3.GetAngle(groundNormal, Vector3.Up);






        if (slopeAngle > resistStartAngle && uphillDir != Vector3.Zero)
        {
            // how much of the acceleration points uphill
           // Vector3 uphillAccel = Vector3.Project(neededAccel, uphillDir);
            Vector3 uphillAccel = uphillDir * Vector3.Dot(neededAccel, uphillDir);
            float uphillDot = Vector3.Dot(uphillAccel.Normal, uphillDir);

            if (uphillAccel.Length > 0f && uphillDot > 0f)
            {
                // Compute how steep the slope is (0–1)

                float steepnessRatio = MathX.LerpInverse(resistStartAngle, resistFullAngle, slopeAngle);

                // Reduce uphill acceleration only
                uphillResistance = MathX.Lerp(1f, 0f, steepnessRatio);


                Vector3 reducedUphill = uphillAccel * uphillResistance;

                reducedUphill *= gripRatio;
                
                Vector3 otherAccel = neededAccel - uphillAccel;

                neededAccel = otherAccel + reducedUphill;
                // Ensure total accel doesn't exceed allowed cap
                neededAccel = neededAccel.ClampLength(maxAccel);
            }
        }
        // --- Apply force ---
        Vector3 forceScale = isGrounded ? new Vector3(1, 1, 1) : new Vector3(1, 1, 0);

        rig.ApplyForce(neededAccel * rig.Mass * forceScale);
    }

    void ManageGroundHugForce()
    {
        HitInformation temp=RaycastFromBody(1.9f);
        if (!isGrounded && !isJumping && Time.Now-lastGroundedTime<0.2f && temp.hit)
        {
            rig.ApplyImpulse(Vector3.Down*groundHugForce);
        }
    }

    private void CalculateJumpForce()
    {
        if (isJumping)
        {
            rig.ApplyForce(jumpForce*(1-(Time.Now-jumpTime)/jumpLength)*jumpNormal);
            
        }
    }


    void ManageJump()
    {
        if(jumpPressed && !isJumping && canJump && Time.Now-jumpTime>jumpInterval)
        {
            if(isGrounded) //this is the start of a jump
            {
                isJumping=true;
                jumpTime=Time.Now;

                rig.Velocity=Vector3.Zero;
                //initial jump should be the strongest
                rig.ApplyForce(jumpForce*2*jumpNormal);
            }
        }
        else if(isJumping && jumpPressed && Time.Now-jumpTime<jumpLength) //add a bit more force as the player holds jump
        {
            float slowDownFactor=1-MathX.Clamp(Time.Now-jumpTime,0.0001f,jumpLength)/jumpLength;

            rig.ApplyForce(jumpForce*slowDownFactor*jumpNormal);
        }
        else
        {
            isJumping=false;
        }
    }


	
	protected override void OnUpdate()
	{

	}



	protected override void OnFixedUpdate()
	{
        ManageJump();
        ManageGroundHugForce();
		ManageSpring();
		ManageFriction();
        ManageMovement();
	}

	//unsure but maybe? check with Unity
	public override void LookAtVector(Vector3 lookDir)
    {
        WorldRotation = Rotation.FromYaw(lookDir.y);
    }

    public override void Move(Vector2 moveInput)
    {
        playerInputs = moveInput;
        
        Vector3 moveDirection = (WorldTransform.Forward * playerInputs.y + WorldTransform.Right * playerInputs.x).Normal;

        desiredDir = moveDirection;



    }

    public override void Jump(bool jumpPress)
    {
        jumpPressed= jumpPress;
    }

    public override float GetSpeed()
    {
        return rig.Velocity.Length;
    }

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Line(raycastPoint.LocalPosition,raycastPoint.LocalPosition+Vector3.Up*-rideHeight);
		Gizmo.Draw.SolidBox(BBox.FromPositionAndSize(raycastPoint.LocalPosition+Vector3.Up*-rideHeight,2));
	}
}