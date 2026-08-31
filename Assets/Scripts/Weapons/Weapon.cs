using BoomerShoot.Character.Components;
using BoomerShoot.WeaponObject.Interfaces;
using UnityEngine;
using UnityEngine.Events;



namespace BoomerShoot.WeaponObject
{

    public enum HeldState
    {
        Grounded,
        Held
    }


    public class Weapon : MonoBehaviour, IUsableEntity, IFirable
    {
        protected WeaponHandler owner;

        public HeldState heldStatus;

        public UnityEvent OnGrab;

        public UnityEvent OnRelease;
        
        public string WeaponName { get {return _weaponName;}}


        [SerializeField] private string _weaponName = "_unknownWeaponString";
        
        public virtual void InitWeapon()
        {

        }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual void Fire()
        {

        }

        public virtual void Equip(WeaponHandler equipper)
        {
            owner = equipper;
            heldStatus = HeldState.Grounded;
            OnGrab!.Invoke();
        }

        public virtual void Unequip()
        {
            owner = null;
            heldStatus = HeldState.Grounded;
            OnRelease!.Invoke();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Interact(InteractionHandler interactor)
        {
            throw new System.NotImplementedException();
        }

        public void PrimaryFire()
        {
            throw new System.NotImplementedException();
        }

        public void SecondaryFire()
        {
            throw new System.NotImplementedException();
        }

        public bool CanFire()
        {
            throw new System.NotImplementedException();
        }

        public void OnZoom()
        {
            throw new System.NotImplementedException();
        }
    }
}