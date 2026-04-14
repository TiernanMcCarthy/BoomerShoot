using Sandbox;

public sealed class Door : Component, iInteractable
{

	[Property] private List<Vector3> targetHeight;

	[Property] private float doorSpeed=50;

	[Property] private GameObject doorVisual;

	int index=0;

	Vector3 lerpTarget;
	public void Interact( WeaponHandler interactor )
	{
		index++;

		if(index>=targetHeight.Count)
		{
			index=0;
		}

		lerpTarget=targetHeight[index];
	}

	protected override void OnStart()
	{
		lerpTarget=targetHeight[0];
	}

	protected override void OnUpdate()
	{
		doorVisual.WorldPosition= Vector3.Lerp(doorVisual.WorldPosition,lerpTarget,doorSpeed*Time.Delta);
	}
}