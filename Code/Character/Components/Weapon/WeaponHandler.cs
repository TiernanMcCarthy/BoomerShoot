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


	Weapon currentWeapon;
	[Property]private IFirable weaponSystem;

	[Property]private IAmmoSystem ammoSystem;

	[Property] private float weaponSwaySpeed=5;

	private int currentWeaponSlot=0;

	
	[Header("Pickup settings")]
	[Property] private float pickupRadius {get;set;} = 150;
	[Property] private GameObject weaponHoldLocation {get;set;}

	[Property] private GameObject playerCamera;

	[Property] private BaseCharacter playerCharacter;

	[Header("Position Sway")]
    [Property] public float amount {get; set;}= 0.02f;
    [Property] public float maxAmount = 0.06f;
    [Property] public float smoothAmount = 6f;

    [Header("Rotation Sway")]
    [Property] public float rotationAmount {get; set;}= 4f;
    [Property] public float maxRotationAmount = 5f;
    [Property] public float smoothRotation = 12f;

    [Header("Look Tilt (Roll)")]
    [Property] public float tiltAmount {get; set;}= 2f;

    private Vector3 initialPosition;
    private Rotation initialRotation;



	protected override void OnStart()
	{
		while(equippedWeapons.Count<2)
		{
			equippedWeapons.Add(new Weapon());
		}

		initialPosition = weaponHoldLocation.LocalPosition;
        initialRotation = weaponHoldLocation.LocalRotation;

	
	}


	public void WeaponPickup(Weapon weapon)
	{
		weaponSystem=weapon.GetComponent<IFirable>();
		ammoSystem=weapon.GetComponent<IAmmoSystem>();
		currentWeapon=weapon;

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

	public string GetWeaponAmmo()
	{
		if(ammoSystem!=null)
		{
			return ammoSystem.GetAmmoStatus();
		}

		return "";
	}



	Vector3 lastForward;
	protected override void OnUpdate()
	{
		WeaponScan();
		HandleWeapon();

        // s&box Mouse.Delta.x is horizontal, y is vertical
        float moveX = -Mouse.Delta.x * amount;
        float moveY = -Mouse.Delta.y * amount;
        
        // 2. Clamp Position Sway
        moveX = moveX.Clamp(-maxAmount, maxAmount);
        moveY = moveY.Clamp(-maxAmount, maxAmount);

        // s&box Space: Y is Horizontal, Z is Vertical.
        Vector3 targetOffset = new Vector3(0, moveX, moveY);

        // 3. Calculate Rotation Sway (Pitch, Yaw, Roll)
        float tiltX = Mouse.Delta.y * rotationAmount;    // Pitch
        float tiltY = -Mouse.Delta.x * rotationAmount;   // Yaw
        float tiltZ = -Mouse.Delta.x * tiltAmount;      // Roll

        // Clamp Rotation values
        tiltX = tiltX.Clamp(-maxRotationAmount, maxRotationAmount);
        tiltY = tiltY.Clamp(-maxRotationAmount, maxRotationAmount);

        Rotation targetRotation = Rotation.From(new Angles(tiltX, tiltY, tiltZ));

        // 4. Apply Smoothing
        // We add the offset to the initialPosition so it stays in the right spot
        weaponHoldLocation.LocalPosition = Vector3.Lerp(weaponHoldLocation.LocalPosition, initialPosition + targetOffset, Time.Delta * smoothAmount);
        weaponHoldLocation.LocalRotation = Rotation.Slerp(weaponHoldLocation.LocalRotation, initialRotation * targetRotation, Time.Delta * smoothRotation);
    

	}
}