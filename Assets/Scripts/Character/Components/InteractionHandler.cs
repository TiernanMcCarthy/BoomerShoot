using BoomerShoot.WeaponObject;
using BoomerShoot.WeaponObject.Interfaces;
using System.Diagnostics;
using UnityEngine;

namespace BoomerShoot.Character.Components
{
    /// <summary>
    /// Generic Interactable Handler for the Player. This Handler can and will pass weapons or equipment to the weapon handler if it finds them
    /// Otherwise, this handler will work with buttons, swapping weapons with characters e.t.c
    /// </summary>
    public class InteractionHandler : MonoBehaviour
    {

        /// <summary>
        /// All weapons and equipment are passed to this object
        /// </summary>
        [SerializeField] private WeaponHandler _weaponHandler;

        /// <summary>
        /// This handler will scan for weapons and equipables in a sphere, interactables should also probably be scanned in a cone from the player
        /// </summary>
        [SerializeField] private float _sphericalScanRange;

        [SerializeField]
        private GameObject _currentEntityGO;

        private IUsableEntity _currentEntity;


        /// <summary>
        /// Public Method for telling this handler to scan, it will pass the results to the appropriate component
        /// </summary>
        /// <param name="scanPos"></param>
        public virtual void ScanForItems(Vector3 scanPos)
        {
            InteractableScan(scanPos);
        }




        /// <summary>
        /// World Space Spherical Physics Scan for Interactables
        /// </summary>
        private void InteractableScan(Vector3 scanPos)
        {

           _currentEntity = null;
           _currentEntityGO = null;
           Collider[] results = Physics.OverlapSphere(scanPos, _sphericalScanRange);

            GameObject closestObject = null;
            float closestDist = 999999;

            IUsableEntity interactable = null;

            for (int i = 0; i < results.Length; i++) //sort for the closest interactable and set this as the target
            {
                float distance= Vector3.Distance(scanPos,results[i].transform.position);

                if (distance < closestDist)
                {

                    interactable = results[i].gameObject.GetComponent<IUsableEntity>();

                    if (interactable != null)
                    {
                        closestObject = results[i].gameObject;
                        closestDist = distance;
                    }
                }
            }

            if (closestObject != null)
            {
                _currentEntity = closestObject.GetComponent<IUsableEntity>();
                _currentEntityGO = closestObject;

            }

        }

        /// <summary>
        /// Interaction function (called whenever a base character chooses to).
        /// Can interact with any IUsableEntity, this includes generics (buttons) or weapons e.t.c
        /// </summary>
        public void UseInteractable()
        {
            if(_currentEntity==null)
            {
                return;
            }

            if (_currentEntity.GetInteractableType() == InteractableType.Weapon)
            {
                Weapon weapon = _currentEntityGO.GetComponent<Weapon>();
                _weaponHandler.TakeWeapon(weapon);
            }
            else if(_currentEntity.GetInteractableType()== InteractableType.GenericInteractable)
            {
                _currentEntity.Interact(this);
            }

        }


    }

}