using UnityEngine;

namespace BoomerShoot.WeaponObject.Interfaces
{
    /// <summary>
    /// Any weapon type needs to implement this interface for the weapon handler to use this
    /// </summary>
    public interface IFirable
    {
        /// <summary>
        /// A weapon's primary fire method
        /// </summary>
        void PrimaryFire();
        
        /// <summary>
        /// Weapons may have a secondary fire method, all weapons should implement this, but this won't do anything
        /// </summary>
        void SecondaryFire();
        

        /// <summary>
        /// Each Weapon should tell the weapon Handler that it is ready to reload
        /// </summary>
        /// <returns></returns>
        bool CanFire();


        /// <summary>
        /// Some weapons should handle zoom
        /// </summary>
        void OnZoom();

    }
}
