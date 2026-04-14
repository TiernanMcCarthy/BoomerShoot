using Sandbox;


public enum HeldState
{
    Grounded,
    Held
}

public interface WeaponEvents : ISceneEvent<WeaponEvents>
{

	public void OnGrab(WeaponHandler grabber);

	public void OnDrop(WeaponHandler dropper);	
}

public class Weapon : Component, WeaponEvents, iInteractable
{

	[Header("Weapon States & Events")]
	//Agent/Player that is holding this object
	[Property] protected WeaponHandler owner {get; set;}

	[Property] public HeldState weaponStatus;
	

	
	protected override void OnUpdate()
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
        weaponStatus = HeldState.Grounded;
        OnGrab(equipper);
    }

	public virtual void Unequip()
    {
        weaponStatus = HeldState.Grounded;
        OnDrop(owner);
		owner = null;
    }

	public void OnGrab( WeaponHandler grabber )
	{
		throw new System.NotImplementedException();
	}

	public void OnDrop( WeaponHandler dropper )
	{
		throw new System.NotImplementedException();
	}

	public void Interact(WeaponHandler interactor)
	{
		Equip(interactor);
	}
}