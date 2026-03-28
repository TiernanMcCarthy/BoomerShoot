using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public virtual Vector2 GetMovementVector()
    {
        return Vector2.zero;
    }
    
    public virtual Vector3 GetLookVector()
    {
        return Vector3.zero;
    }

    public virtual bool GetJumpState()
    {
        return false;
    }

    public virtual void PollInputs()
    {
        
    }
}
