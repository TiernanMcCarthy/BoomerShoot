using Sandbox;

public sealed class PlayerWeaponHandler : WeaponHandler
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

	private string weaponStringReadout ="";


	protected override void OnStart()
	{
		equippedWeapons= new List<Weapon>();
		initialPosition = weaponHoldLocation.LocalPosition;
        initialRotation = weaponHoldLocation.LocalRotation;
	}


	public override List<string> GetWeaponStrings()
	{
		List<string> weaponList= new List<string>();

		for(int i=0; i<equippedWeapons.Count;i++)
		{
			if(equippedWeapons[i]!=null)
			{
				weaponList.Add(equippedWeapons[i].WeaponName);
			}
		}
		return weaponList;
	}

	public override string InteractableStringReadout()
	{
		return weaponStringReadout;
	}


	public override void WeaponPickup(Weapon weapon)
	{
		weaponSystem=weapon.GetComponent<IFirable>();
		ammoSystem=weapon.GetComponent<IAmmoSystem>();

		bool dropCurrentWeapon=false;

		if(equippedWeapons.Count<2)
		{
			equippedWeapons.Add(weapon);
			currentWeaponSlot=equippedWeapons.Count-1;
		}
		else
		{
			equippedWeapons[currentWeaponSlot]=weapon;
			dropCurrentWeapon=true;
		}

		if(currentWeapon!=null)
		{

			if(dropCurrentWeapon)
			{
				currentWeapon.Unequip();
			}
			else
			{
				currentWeapon.GameObject.Enabled=false;
			}
		}
		
		currentWeapon=weapon;

		weapon.GameObject.SetParent(weaponHoldLocation);
		weapon.LocalPosition= new Vector3(0,0,0);
		weapon.LocalRotation= new Rotation();
	}

	private void DropCurrentWeapon()
	{
		if(currentWeapon!=null)
		{
			currentWeapon.Unequip();
			currentWeapon=null;

		}
	}


	private void WeaponScan()
	{
		var trace =Scene.PhysicsWorld.Trace.Sphere(pickupRadius,WorldPosition,WorldPosition).RunAll();

		GameObject closestObject=null;
		float closestDist=999999;

		weaponStringReadout="";

		iInteractable interactable=null;

		for(int i=0; i<trace.Length; i++)
		{
			if(trace[i].Distance<closestDist)
			{

				GameObject target=null;
				interactable=trace[i].Body.GameObject.GetComponent<iInteractable>();
				if(interactable!=null)
				{
					target=trace[i].Body.GameObject;
				}
				if(target!=null)
				{
					if(equippedWeapons.Count>0)
					{
						string targetString=interactable.GetItemString();

						bool canReloadAGun=false;

						Weapon castAsWeapon=null;

						Weapon reloadWeapon=null;

						foreach(Weapon weapon in equippedWeapons)
						{
							if(weapon!=null)
							{

								if(targetString==weapon.GetItemString())
								{
									castAsWeapon=target.GetComponent<Weapon>();
									canReloadAGun=true;
									reloadWeapon=weapon;
									break;
								}
							}
						}

						//weapon Ammo Code

						

						if(canReloadAGun)
						{

							reloadWeapon.ResupplyWeapon(castAsWeapon);
							break;
						}
					}


					closestObject=target;
					closestDist=trace[i].Distance;
				}
			}
		}

		if(closestObject!=null)
		{
			weaponStringReadout=string.Format("Press E to {0} {1}", interactable.GetActionString(),interactable.GetItemString());


			if(Input.Released("Use"))
			{
				interactable.Interact(this); 
			}
		}

	}

	private void HandleWeapon()
	{

		if(Input.Down("Reload"))
		{
			if(ammoSystem!=null)
			{
				ammoSystem.TryReload();
			}
		}
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

	public override string GetWeaponAmmo()
	{
		if(ammoSystem!=null)
		{
			return ammoSystem.GetAmmoStatus();
		}

		return "";
	}

	private void DisableWeapons()
	{
		for(int wep=0; wep<equippedWeapons.Count; wep++)
		{
			Weapon weapon= equippedWeapons[wep];

			if(weapon!=null)
			{
				weapon.GameObject.Enabled=false;
			}
		}
	}


	private void EnableWeapon(int i)
	{
		Weapon weapon= equippedWeapons[i];

		weapon.GameObject.Enabled=true;
	}

	private void ManageWeaponInventory()
	{
		if(Input.Released("SwapWeapon"))
		{
			if(equippedWeapons.Count>1)
			{
				
				equippedWeapons[currentWeaponSlot].GameObject.Enabled=false;

				currentWeaponSlot++;

				if(currentWeaponSlot>=equippedWeapons.Count)
				{
					currentWeaponSlot=0;
				}

				currentWeapon=equippedWeapons[currentWeaponSlot];

				currentWeapon.GameObject.Enabled=true;

				weaponSystem=currentWeapon.GetComponent<IFirable>();
				ammoSystem=currentWeapon.GetComponent<IAmmoSystem>();

			}
			

		}
	}

	Vector3 lastForward;
	protected override void OnUpdate()
	{
		WeaponScan();
		HandleWeapon();
		ManageWeaponInventory();


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