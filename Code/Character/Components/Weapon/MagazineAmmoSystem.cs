using Sandbox;

public sealed class MagazineAmmoSystem : Component, IAmmoSystem
{

	[Header("Ammo Settings")]
	[Property] private int magazineSize {get; set;} = 10;

	[Property] private int maxAmmo {get; set;} =6;

	[Property] public int reserveAmmo	{get;private set;}

	int currentMagCount;

	[Property] private float reloadSpeed;

	bool isReloading=false;

	private float reloadTime;

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
		return false;
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