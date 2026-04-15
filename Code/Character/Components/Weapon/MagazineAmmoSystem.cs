using Sandbox;

public sealed class MagazineAmmoSystem : Component, IAmmoSystem
{
	public bool CanFire()
	{
		return true;
	}

	public string GetAmmoStatus()
	{
		return "";
		//throw new System.NotImplementedException();
	}

	public bool IsEmpty()
	{
		return false;
		//throw new System.NotImplementedException();
	}

	protected override void OnUpdate()
	{

	}
}