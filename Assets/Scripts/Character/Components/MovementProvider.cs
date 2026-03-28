

using UnityEngine;


public class MovementProvider : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public virtual void LookAtVector(Vector3 lookDir)
    {
        
    }
    
    public virtual void Move(Vector2 moveInput)
    {
        
    }

    public virtual void Jump(bool jumpPressed)
    {
        
    }

    public virtual float GetSpeed()
    {
        return 0;
    }



}