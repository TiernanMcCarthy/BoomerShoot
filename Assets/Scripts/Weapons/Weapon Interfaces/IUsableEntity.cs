using UnityEngine;


namespace BoomerShoot.Weapon.Interfaces
{
    public interface IUsableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactor"></param>
        public void Interact(WeaponHandler interactor);

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