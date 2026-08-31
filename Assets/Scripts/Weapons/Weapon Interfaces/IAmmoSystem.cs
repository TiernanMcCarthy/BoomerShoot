using UnityEngine;

namespace BoomerShoot.WeaponObject.Interfaces
{
    /// <summary>
    /// Ammo System interface for custom Ammo Components, a Plasma weapon will work differently to a magazine weapon
    /// </summary>
    public interface IAmmoSystem
    {
        /// <summary>
        /// All weapons will and should return a projectile count, this is just the total shots it can fire
        /// </summary>
        /// <returns></returns>
        int GetProjectileCount();

        /// <summary>
        /// Resupply this ammo system to carry more rounds
        /// </summary>
        /// <param name="targetWeapon"></param>
        /// <returns></returns>
        public bool ResupplyWeapon(Weapon targetWeapon);

        /// <summary>
        /// Return the total stored ammo
        /// </summary>
        /// <returns></returns>
        public int GetTotalAmmo();

        /// <summary>
        /// Try and consume these bullets (if it possible to fire?)
        /// </summary>
        /// <param name="rounds"></param>
        /// <returns></returns>
        public int TryConsumeRounds(int rounds);

        /// <summary>
        /// Each weapon will have different firing patterns and abilities, this will function differently per gun
        /// </summary>
        /// <returns></returns>
        bool CanFire();

        /// <summary>
        /// Try for a reload, if the weapon is busy, or just empty, this won't happen
        /// </summary>
        /// <returns></returns>
        bool TryReload();


        /// <summary>
        /// You should be able to cancel a reload by swapping weapon, meleeing, e.t.c
        /// </summary>
        void CancelReload();

        /// <summary>
        /// This can and will only happen if the weapon can reload, and it will take place over several seconds unless it is interupted
        /// </summary>
        void ReloadWeapon();

        /// <summary>
        /// Just says if the weapon is empty, makes it impossible to pick up and saves on testing if we can take ammo
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();

        /// <summary>
        /// Remove a bullet from the (chamber)/bullet pool
        /// </summary>
        void Deplete();

        /// <summary>
        /// Return how many rounds out of X we have (more complex interactions later)
        /// </summary>
        /// <returns></returns>
        public string GetAmmoStatus();

        /// <summary>
        /// Return Projectile Prefab to compare this ammo's prefab with the target weapon's ammo prefab also
        /// </summary>
        /// <returns></returns>
        public Projectile GetAmmoType();

    }
}