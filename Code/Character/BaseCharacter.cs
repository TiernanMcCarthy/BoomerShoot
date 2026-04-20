using Sandbox;


	public interface IPlayerEvent
	{
	void OnDeath( BaseCharacter player );
	}

/// <summary>
/// All Characters inherit and work from this class
/// Composition of extra functionality
/// </summary>
public class BaseCharacter : Component ,IPlayerEvent
{

	public MovementProvider movementProvider { get; private set; }
    
    public InputProvider inputProvider { get; private set; }

	public HealthComponent healthComponent;

	private WeaponHandler weaponHandler;

	//No clue how to use or implement yet :)
	public void OnDeath( BaseCharacter player )
	{
		throw new System.NotImplementedException();
	}

	protected override void OnAwake()
	{
		movementProvider=GetComponent<MovementProvider>();
        inputProvider=GetComponent<InputProvider>();
		weaponHandler=GetComponent<WeaponHandler>();
		healthComponent=GetComponent<HealthComponent>();
	}

	protected override void OnUpdate()
	{
		inputProvider.PollInputs();
        movementProvider.LookAtVector(inputProvider.GetLookVector());
        movementProvider.Move(inputProvider.GetMovementVector());
        movementProvider.Jump(inputProvider.GetJumpState());
	}

	public string GetWeaponAmmo()
	{
		if(weaponHandler!=null)
		{
			return weaponHandler.GetWeaponAmmo();
		}
		return "";
	}

	public string GetHealth()
	{
		return healthComponent.GetHealthString();
	}

	public float GetMaxHealth()
	{
		return healthComponent.maxHealth;
	}

	public float GetShieldStrength()
	{
		return healthComponent.GetHealth();
	}

	public string GetInteractionString()
	{
		return weaponHandler.InteractableStringReadout();
	}
}