using BoomerShoot.WeaponObject;
using UnityEngine;
namespace BoomerShoot.Character.Components
{
    public class WeaponHandler : MonoBehaviour
    {


        public void TakeWeapon(Weapon weapon)
        {
            Debug.Log(weapon.gameObject.name);
        }

    }
}
