using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.Physics;


public interface iInteractable
{
	public void Interact(WeaponHandler interactor);
}

/// <summary>
/// The Weapon Handler exists as a component to interact with and fire weapons, or to pick up and drop weapons
/// </summary>
public class WeaponHandler : Component
{

	[Header("Weapon Settings")]
	[Property] private List<Weapon> equippedWeapons {get; set;}

	[Property]private IFirable weaponSystem;

	[Property]private IAmmoSystem ammoSystem;

	private int currentWeaponSlot=0;

	
	[Header("Pickup settings")]
	[Property] private float pickupRadius {get;set;} = 150;
	[Property] private GameObject weaponHoldLocation {get;set;}

	[Property] private GameObject playerCamera;

	private float weaponZangle;




	protected override void OnStart()
	{
		while(equippedWeapons.Count<2)
		{
			equippedWeapons.Add(new Weapon());
		}
	}

	public void ProvideWeaponZRotation(float rot)
	{
		weaponZangle=rot;
	}

	public void WeaponPickup(Weapon weapon)
	{
		weaponSystem=weapon.GetComponent<IFirable>();
		ammoSystem=weapon.GetComponent<IAmmoSystem>();

		weapon.GameObject.SetParent(weaponHoldLocation);
		weapon.LocalPosition= new Vector3(0,0,0);
		weapon.LocalRotation= new Rotation();
	}


	private void WeaponScan()
	{
		var trace =Scene.PhysicsWorld.Trace.Sphere(pickupRadius,WorldPosition,WorldPosition).RunAll();

		GameObject closestObject=null;
		float closestDist=999999;

		for(int i=0; i<trace.Length; i++)
		{
			if(trace[i].Distance<closestDist)
			{

				GameObject target=null;
				if(trace[i].Body.GameObject.GetComponent<iInteractable>()!=null)
				{
					target=trace[i].Body.GameObject;
				}
				if(target!=null)
				{
					closestObject=target;
					closestDist=trace[i].Distance;
				}
			}
		}

		if(closestObject!=null)
		{
			Log.Info(closestObject);
			
			if(Input.Released("Use"))
			{
				closestObject.GetComponent<iInteractable>().Interact(this); 
			}
		}

	}

	private void HandleWeapon()
	{
		if(Input.Down("Attack1"))
		{
			if(weaponSystem!=null && ammoSystem!=null)
			{
				if(ammoSystem.CanFire() && weaponSystem.CanFire())
				{
					weaponSystem.PrimaryFire();
				}
			}
		}
	}

	protected override void OnUpdate()
	{
		WeaponScan();
		HandleWeapon();

		Vector3 forward=weaponHoldLocation.LocalRotation.Forward;
		forward.z=weaponZangle;
		weaponHoldLocation.LocalRotation= Rotation.FromPitch(playerCamera.LocalRotation.Angles().pitch*0.2f);
	}
}