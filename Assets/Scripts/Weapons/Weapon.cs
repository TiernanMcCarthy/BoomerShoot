using UnityEngine;
using UnityEngine.Events;

public enum HeldState
{
    Grounded,
    Held
}
public class Weapon : MonoBehaviour
{
    protected WeaponHandler owner;

    public HeldState heldStatus;

    public UnityEvent OnGrab;
    
    public UnityEvent OnRelease;

    public virtual void InitWeapon()
    {
        
    }
    
    public virtual bool IsEmpty()
    {
        return false;
    }

    public virtual void Fire()
    {
        
    }

    public virtual void Equip(WeaponHandler equipper)
    {
        owner = equipper;
        heldStatus = HeldState.Grounded;
        OnGrab!.Invoke();
    }

    public virtual void Unequip()
    {
        owner = null;
        heldStatus = HeldState.Grounded;
        OnRelease!.Invoke();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
