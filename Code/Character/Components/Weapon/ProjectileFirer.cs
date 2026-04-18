using Sandbox;

public sealed class ProjectileFirer : Component, IFirable
{
	
	[Header("Weapon Properties")]
	[Property] private float fireRate {get; set;} = 0.2f; //fire speed in seconds

	[Property] private float burstSize {get; set;} =1;

	[Property] private float projectileSpeed=50;

	[Property] private Rigidbody projectile;

	[Property] private SoundFile fireSound;

	[Property] private SoundEvent fireEvent;


	private List<Rigidbody> projectileList;


	Weapon attachedWeapon;
	IAmmoSystem ammoSystem;

	private float fireTime=0;

	int currentProjectile=0;

	public bool CanFire()
	{
		if(ammoSystem.CanFire() && Time.Now-fireTime>fireRate)
		{
			return true;
		}
		return false;
	}
	public void PrimaryFire()
	{
		if(currentProjectile>=projectileList.Count)
		{
			currentProjectile=0;
		}

		ammoSystem.Deplete();

		Rigidbody assignedProj=projectileList[currentProjectile];

		assignedProj.Velocity=Vector3.Zero;

		assignedProj.GameObject.Enabled=true;

		assignedProj.WorldPosition= attachedWeapon.firePosition.WorldPosition;

		assignedProj.WorldRotation=attachedWeapon.WorldRotation;

		assignedProj.ApplyImpulse(assignedProj.WorldTransform.Forward*projectileSpeed);

		currentProjectile++;
		fireTime=Time.Now;

		Sound.Play(fireEvent,attachedWeapon.firePosition.WorldPosition);

		
	}

	public void OnZoom()
	{
		//throw new System.NotImplementedException();
	}

	protected override void OnStart()
	{
		ammoSystem=GetComponent<IAmmoSystem>();

		attachedWeapon=GetComponent<Weapon>();

		fireSound.Preload();


		//create bullet buffer
		projectileList= new List<Rigidbody>();
		for(int i=0; i< ammoSystem.GetProjectileCount(); i++)
		{
			projectileList.Add(projectile.GameObject.Clone().GetComponent<Rigidbody>());
			projectileList[i].GameObject.Enabled=false;
			projectileList[i].EnhancedCcd=true;
		}




	}

	protected override void OnUpdate()
	{
		
	}
}