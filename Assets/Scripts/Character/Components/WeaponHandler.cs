using System;
using System.Collections.Generic;
using BoomerShoot.WeaponObject;
using BoomerShoot.WeaponObject.Interfaces;
using UnityEngine;
namespace BoomerShoot.Character.Components
{
    /// <summary>
    /// The Weapon Handler exists as a component to interact with and fire weapons, or to pick up and drop weapons
    /// </summary>
    public class WeaponHandler : MonoBehaviour
    {
        
        [Header("Weapon Settings")]
        [SerializeField] private List<Weapon> _equippedWeapons;

        [Header("PickUp Settings")] 

        [SerializeField] private Transform _weaponHoldLocation;
        
        [SerializeField] private BaseCharacter _attachedCharacter;
        
        // private weapon settings
        private Weapon _currentWeapon;

        private int _currentWeaponSlot = 0;

        private IFirable _weaponSystem;
        
        private IAmmoSystem _ammoSystem;


        public void Start()
        {
            if (_equippedWeapons == null)
            {
                _equippedWeapons = new List<Weapon>();
            }
        }
        
        /// <summary>
        /// Return all contained weapons as a list of strings
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetWeaponStrings()
        {
            List<string> weaponList= new List<string>();

            for(int i=0; i<_equippedWeapons.Count;i++)
            {
                if(_equippedWeapons[i]!=null)
                {
                    weaponList.Add(_equippedWeapons[i].WeaponName);
                }
            }
            return weaponList;
        }

        /// <summary>
        /// Return the current weapon string or nothing if nothing is equipped
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentWeaponString()
        {
            if (_currentWeapon != null)
            {
                return _currentWeapon.WeaponName;
            }
            return "";
        }
        
        /// <summary>
        /// Checks if the provided weapon is valid and then adds it to the weapon list, swapping out if necessary.
        /// This also parents the new object
        /// </summary>
        /// <param name="weapon"></param>
        public void TakeWeapon(Weapon weapon)
        {
            _weaponSystem=weapon.GetComponent<IFirable>();
            _ammoSystem=weapon.GetComponent<IAmmoSystem>();

            bool dropCurrentWeapon=false;

            if(_equippedWeapons.Count<2)
            {
                _equippedWeapons.Add(weapon);
                _currentWeaponSlot=_equippedWeapons.Count-1;
            }
            else
            {
                _equippedWeapons[_currentWeaponSlot]=weapon;
                dropCurrentWeapon=true;
            }
            
            if(_currentWeapon!=null)
            {

                if(dropCurrentWeapon)
                {
                    _currentWeapon.Unequip();
                }
                else
                {
                    _currentWeapon.gameObject.SetActive(false);
                }
            }
		
            _currentWeapon=weapon;

            weapon.transform.SetParent(_weaponHoldLocation);
            weapon.transform.localPosition= new Vector3(0,0,0);
            weapon.transform.localRotation = Quaternion.identity;
        }
        
        /// <summary>
        /// Drop the current weapon and inform the weapon of this
        /// </summary>
        protected virtual void DropCurrentWeapon()
        {
            if(_currentWeapon!=null)
            {
                _currentWeapon.Unequip();
                _currentWeapon=null;

            }
        }

    }
    
    
}
