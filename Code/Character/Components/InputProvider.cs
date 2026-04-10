using Sandbox;

public class InputProvider : Component
{
	public virtual Vector2 GetMovementVector()
    {
        return Vector2.Zero;
    }
    
    public virtual Vector3 GetLookVector()
    {
        return Vector3.Zero;
    }

    public virtual bool GetJumpState()
    {
        return false;
    }

    public virtual void PollInputs()
    {
        
    }
}