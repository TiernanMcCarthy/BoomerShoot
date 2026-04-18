using Sandbox;

public class HealthComponent : Component
{
	[Header("Character Health Settings")]
	[Property] public float maxHealth { get; private set;}
	[Property] private float health;


	protected override void OnUpdate()
	{

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