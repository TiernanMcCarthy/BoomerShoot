using BoomerShoot.Character.Components;
using BoomerShoot.WeaponObject.Interfaces;
using UnityEngine;
using UnityEngine.Events;


namespace BoomerShoot.Interactables
{
    /// <summary>
    /// Incredibly simple button that can be interacted with in the scene, invoking its OnSelection event whenever it's pressed
    /// </summary>
    public class InteractableButton : MonoBehaviour, IUsableEntity
    {
        public UnityEvent OnSelection;

        public void Interact(InteractionHandler interactor)
        {
            OnSelection.Invoke();
        }

        public InteractableType GetInteractableType()
        {
            return InteractableType.GenericInteractable;
        }

    }
}