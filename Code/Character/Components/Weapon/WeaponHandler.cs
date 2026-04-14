using System.Diagnostics;
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

	
	[Header("Pickup settings")]
	[Property] private float pickupRadius {get;set;} = 150;
	[Property] private GameObject weaponHoldLocation {get;set;}



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

	protected override void OnUpdate()
	{
		WeaponScan();
	}
}