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
	int GetProjectileCount();

	public bool ResupplyWeapon(Weapon targetWeapon);

	public int GetTotalAmmo();

	public int TryConsumeRounds(int rounds);

	bool CanFire();

	bool TryReload();

	void CancelReload();

	void ReloadWeapon();

	bool IsEmpty();

	void Deplete();

	public string GetAmmoStatus();

	public string GetAmmoType();


}

public class Weapon : Component, iInteractable
{


	[Header("Weapon States & Events")]

 	[Property] public string WeaponName {get; private set;}
	//Agent/Player that is holding this object
	[Property] protected WeaponHandler owner {get; set;}

	[Property] public HeldState weaponStatus;

	[Header("Cosmetic Elements")]
	[Property] private GameObject worldSpaceModel {get; set;}
	[Property] private GameObject playerHeldModel;

	[Property] public GameObject firePosition {get; private set;}
	
	private Rigidbody rig;

	[Property]private IAmmoSystem ammoSystem;


	protected override void OnStart()
	{
		rig=GetComponent<Rigidbody>();
		ammoSystem=GetComponent<IAmmoSystem>();
	}

	
	public string GetItemString()
	{
		return WeaponName;
	}

	public string GetActionString()
	{
		return "Pickup";
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

		GameObject.SetParent(null);

		//toggle cosmetic models
		worldSpaceModel.Enabled=true;
		playerHeldModel.Enabled=false;
	}

	public virtual void Equip(WeaponHandler equipper)
    {
		List<string> currentWeapons=equipper.GetWeaponStrings();

		foreach(string s in currentWeapons)
		{
			if(s==WeaponName)
			{
				return;
			}
		}

        owner = equipper;

        weaponStatus = HeldState.Grounded;
		EnableHeldBehaviour();
		owner.WeaponPickup(this);

    }

	public virtual void Unequip()
    {
		GameObject.Enabled=true;
        weaponStatus = HeldState.Grounded;
		owner = null;
		EnableRigidbodyBehaviour();

    }

	public virtual bool ResupplyWeapon(Weapon target)
	{
		return ammoSystem.ResupplyWeapon(target);
	}

	public virtual string GetWeaponAmmoType()
	{
		return ammoSystem.GetAmmoType();
	}

	

	public void Interact(WeaponHandler interactor)
	{
		Equip(interactor);
	}
}