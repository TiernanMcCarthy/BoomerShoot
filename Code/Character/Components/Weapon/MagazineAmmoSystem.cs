using Sandbox;

public sealed class MagazineAmmoSystem : Component, IAmmoSystem
{

	[Header("Ammo Settings")]

	[Property] public string weaponAmmoType {get; set;} = "REASSIGN";

	[Property] private int magazineSize {get; set;} = 10;

	[Property] private int maxAmmo {get; set;} =6;

	[Property] public int reserveAmmo	{get;private set;}

	[Property] int currentMagCount;

	[Property] private float reloadSpeed;

	bool isReloading=false;

	private float reloadTime;


	public string GetAmmoType()
	{
		return weaponAmmoType;
	}

	public int GetTotalAmmo()
	{
		int total=reserveAmmo+currentMagCount;

		return total;
	}

	public int TryConsumeRounds(int rounds)
	{
		if(rounds<reserveAmmo) //if the reserve is enough
		{
			reserveAmmo-=rounds;
			return rounds;
		}

		if(rounds<reserveAmmo + currentMagCount) //if the reserve + current mag is enough
		{
			currentMagCount=currentMagCount-(rounds-reserveAmmo);
			reserveAmmo=0;
			return rounds;
		}

		
		//deplete the whole gun
		GameObject.Destroy();
		int returnTarget=reserveAmmo+currentMagCount;
		currentMagCount=0;
		reserveAmmo=0;
		return returnTarget;
	}

	public bool ResupplyWeapon(Weapon targetWeapon)
	{
		//deplete ammo from weapon
		if(targetWeapon.GetWeaponAmmoType()==weaponAmmoType)
		{
		    IAmmoSystem targetAmmo=targetWeapon.GetComponent<IAmmoSystem>();

			reserveAmmo+=targetAmmo.TryConsumeRounds(maxAmmo-(reserveAmmo));

			//if(total.)
			
		}
		
		//Destroy Weapon
		return false;
	}

	public bool CanFire()
	{
		if(isReloading)
		{
			return false;
		}
		return currentMagCount>0;
	}

	public string GetAmmoStatus()
	{
		return  string.Format("{0} / {1}",currentMagCount.ToString(),reserveAmmo);
		//throw new System.NotImplementedException();
	}

	public int GetProjectileCount()
	{
		return magazineSize;
	}

	public bool IsEmpty()
	{
		return currentMagCount>0 && reserveAmmo>0;
		//throw new System.NotImplementedException();
	}

	public void CancelReload()
	{
		isReloading=false;
	}

	public bool TryReload()
	{

		if(isReloading)
		{
			return false;
		}
		reserveAmmo+=currentMagCount;

		currentMagCount=0;

		if(reserveAmmo>=magazineSize ||reserveAmmo>0 ) //reload normally
		{
			reloadTime=Time.Now;
			isReloading=true;
			return true;
		}

		//can't reload

		isReloading=false;
		return false;
	}

	public void ReloadWeapon()
	{
		if(isReloading && Time.Now-reloadTime>=reloadSpeed)
		{	
			if(reserveAmmo>=magazineSize) //reload normally
			{
				currentMagCount=magazineSize;
				reserveAmmo-=magazineSize;
			}
			else if(reserveAmmo>0) //reload what is left
			{
				currentMagCount=reserveAmmo;
				reserveAmmo=0;
			}

			isReloading=false;

		}
	}

	public void Deplete()
	{
		currentMagCount-=1;
	}

	protected override void OnUpdate()
	{
		ReloadWeapon();
	}

	protected override void OnStart()
	{
		currentMagCount=magazineSize;
	}
}