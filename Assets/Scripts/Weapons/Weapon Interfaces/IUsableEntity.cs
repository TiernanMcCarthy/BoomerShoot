using BoomerShoot.Character.Components;
using UnityEngine;


namespace BoomerShoot.WeaponObject.Interfaces
{

    public enum InteractableType
    {
        Weapon,
        Equipment,
        GenericInteractable
    }

    public interface IUsableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactor"></param>
        public void Interact(InteractionHandler interactor);

        public InteractableType GetInteractableType()
        {
            return InteractableType.Weapon;
        }

        /// <summary>
        /// Name of the item, e.g. Assault Rifle, button, e.t.c
        /// </summary>
        /// <returns></returns>
        string GetItemString()
        {
            return "Undefined_Type";
        }

        /// <summary>
        /// Action e.g. "Pickup", "Activate"
        /// </summary>
        /// <returns></returns>
        string GetActionString()
        {
            return "Undefined Action";
        }

        

    }
}