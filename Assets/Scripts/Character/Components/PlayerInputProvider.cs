

using System;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerInputProvider : InputProvider
{
    PlayerActions playerActions;
    
    private Vector2 inputVector;
    
    private Vector3 lookDir;
    
    private bool jumpPressed;
    
    

    [Header("Camera Settings")] 
    [SerializeField] private float mouseSensX = 5;
    [SerializeField] private float mouseSensY = 5;
    [SerializeField] private float _controllerSensitivity = 5;
    
    [SerializeField] private Transform camLocation;
    [SerializeField] private Transform cameraReference;
    
    private void Start()
    {
        playerActions = new PlayerActions();
        playerActions.Enable();
        cameraReference.transform.SetParent(null);
        
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
        inputVector.x = playerActions.FirstPerson.Horizontal.ReadValue<float>();
        inputVector.y = playerActions.FirstPerson.Vertical.ReadValue<float>();

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Scale these by sensitivity.
        lookDir.y += mouseDelta.x * mouseSensX; 
        lookDir.x -= mouseDelta.y * mouseSensY; 
        
        if(Gamepad.current.IsActuated())
        {
            lookDir.y+= playerActions.FirstPerson.LookHorizontal.ReadValue<float>() *_controllerSensitivity;
            lookDir.x -= playerActions.FirstPerson.LookVertical.ReadValue<float>() * _controllerSensitivity;    
        }

        // Clamp pitch to prevent the camera flipping upside down
        lookDir.x = Mathf.Clamp(lookDir.x, -80f, 80f);

        lookDir = new Vector3(lookDir.x, lookDir.y, 0);

        jumpPressed = playerActions.FirstPerson.Jump.IsPressed();
    }

    private void LateUpdate()
    {
        camLocation.transform.localRotation = Quaternion.Euler(new Vector3(lookDir.x, 0, 0));
        cameraReference.transform.forward=camLocation.transform.forward;
        cameraReference.transform.position=camLocation.transform.position;
    }

    public override Vector2 GetMovementVector()
    {
        return inputVector;
    }
}