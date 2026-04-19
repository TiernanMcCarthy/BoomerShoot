using System.Runtime.CompilerServices;
using Sandbox;

public class HealthComponent : Component
{

	[Header("Shield Settings")]
	[Property] public float maxShieldStrength {get; private set;}

	[Property] private float currentShieldStrength {get; set;}

	[Property] private float shieldRechargeInterval =4;

	[Property] private float shieldRechargeSpeed {get; set;}


	[Header("Character Health Settings")]
	[Property] public float maxHealth { get; private set;}
	[Property] private float health;


	protected override void OnUpdate()
	{

	}

	protected override void OnStart()
	{
		currentShieldStrength=maxShieldStrength;
		health=maxHealth;
	}

	public virtual string GetHealthString()
	{
		return health.ToString();
	}

	public virtual float GetHealth()
	{
		return health;
	}
}