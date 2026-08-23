using UnityEngine;

public class WeaponHandler : MonoBehaviour
{


    public void TakeWeapon(Weapon weapon)
    {
        Debug.Log(weapon.gameObject.name);
    }

}
