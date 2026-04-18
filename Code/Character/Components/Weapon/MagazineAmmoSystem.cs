using Sandbox;

public sealed class MagazineAmmoSystem : Component, IAmmoSystem
{

	[Header("Ammo Settings")]
	[Property] private int magazineSize {get; set;} = 10;

	[Property] private int maxMagazines {get; set;} =6;

	int currentMagCount;

	[Property] private float reloadSpeed;
	public bool CanFire()
	{
		return currentMagCount>0;
	}

	public string GetAmmoStatus()
	{
		return currentMagCount.ToString();
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

	public void Deplete()
	{
		currentMagCount-=1;
	}

	protected override void OnUpdate()
	{

	}

	protected override void OnStart()
	{
		currentMagCount=magazineSize;
	}
}