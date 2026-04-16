using Sandbox;

public sealed class ProjectileFirer : Component, IFirable
{
	
	public bool CanFire()
	{
		return false;
	}
	public void PrimaryFire()
	{
		Log.Info("I'm firing me laser");
	}

	public void OnZoom()
	{
		//throw new System.NotImplementedException();
	}

	protected override void OnUpdate()
	{

	}
}