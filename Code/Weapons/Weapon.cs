using Sandbox;


public enum HeldState
{
    Grounded,
    Held
}

public interface IFirable
{
	
	void PrimaryFire();

	bool CanFire();

	void OnZoom();

}

public interface IAmmoSystem
{
	
	bool CanFire();

	bool IsEmpty();

	string GetAmmoStatus();
}

public class Weapon : Component, iInteractable
{

	[Header("Weapon States & Events")]
	//Agent/Player that is holding this object
	[Property] protected WeaponHandler owner {get; set;}

	[Property] public HeldState weaponStatus;

	[Header("Cosmetic Elements")]
	[Property] private GameObject worldSpaceModel {get; set;}
	[Property] private GameObject playerHeldModel;
	
	private Rigidbody rig;


	protected override void OnStart()
	{
		rig=GetComponent<Rigidbody>();
	}

	protected override void OnUpdate()
	{

	}

	private void EnableHeldBehaviour()
	{
		rig.Enabled=false;

		//toggle cosmetic models
		worldSpaceModel.Enabled=false;
		playerHeldModel.Enabled=true;


	}

	private void EnableRigidbodyBehaviour()
	{
		rig.Enabled=true;

		//toggle cosmetic models
		worldSpaceModel.Enabled=true;
		playerHeldModel.Enabled=false;
	}

	public virtual void Equip(WeaponHandler equipper)
    {
        owner = equipper;
        weaponStatus = HeldState.Grounded;
		EnableHeldBehaviour();
		owner.WeaponPickup(this);

    }

	public virtual void Unequip()
    {
        weaponStatus = HeldState.Grounded;
		owner = null;
		EnableRigidbodyBehaviour();

    }

	public void Interact(WeaponHandler interactor)
	{
		Equip(interactor);
	}
}