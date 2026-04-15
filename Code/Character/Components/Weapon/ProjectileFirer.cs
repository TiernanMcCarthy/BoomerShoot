using Sandbox;

public sealed class ProjectileFirer : Component, IFirable
{
	

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