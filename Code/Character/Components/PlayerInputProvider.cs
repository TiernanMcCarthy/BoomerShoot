using Sandbox;

public class PlayerInputProvider : InputProvider
{
    
    private Vector2 inputVector;
    
    private Vector3 lookDir;
    
    private bool jumpPressed;
    
    

    [Header("Camera Settings")] 
    [Property] private float mouseSensX {get; set;}=5;
    [Property] private float mouseSensY = 5;
    [Property] private GameObject camLocation;
    [Property] private GameObject cameraReference;
    
    private void Start()
    {
       // playerActions = new PlayerActions();
        //playerActions.Enable();
        cameraReference.SetParent(null);
        
    }

    public override Vector3 GetLookVector()
    {
        return lookDir;
    }

    public override bool GetJumpState()
    {
        return jumpPressed;
    }

    public override void PollInputs()
    {
        //inputVector.x = playerActions.FirstPerson.Horizontal.ReadValue<float>();
        //inputVector.y = playerActions.FirstPerson.Vertical.ReadValue<float>();

		inputVector=Vector2.Zero;

		if(Input.Down("Forward"))
		{
			inputVector.y=1;
		}
		else if( Input.Down("Backward"))
		{
			inputVector.y=-1;
		}

		if(Input.Down("Right"))
		{
			inputVector.x=1;
		}
		else if( Input.Down("Left"))
		{
			inputVector.x=-1;
		}
        Vector2 mouseDelta = Mouse.Delta;

        // Scale these by sensitivity. (Usually delta doesn't need Time.deltaTime)
        lookDir.y -= mouseDelta.x * mouseSensX; 
        lookDir.x += mouseDelta.y * mouseSensY; // Inverted usually feels 'natural'

        // Clamp pitch to prevent the camera flipping upside down
        lookDir.x = MathX.Clamp(lookDir.x, -80f, 80f);

        lookDir = new Vector3(lookDir.x, lookDir.y, 0);

       // jumpPressed = playerActions.FirstPerson.Jump.IsPressed();
    }

	//no equivalent of late update, lets try here
	protected override void OnPreRender()
	{
		camLocation.LocalRotation = Rotation.FromPitch(lookDir.x);
		cameraReference.WorldRotation = camLocation.WorldRotation;
		cameraReference.WorldPosition = camLocation.WorldPosition;
	}
	
    public override Vector2 GetMovementVector()
    {
        return inputVector;
    }
}