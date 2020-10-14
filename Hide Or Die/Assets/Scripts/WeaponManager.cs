using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponAbstract currentWeapon = null;
    public WeaponAbstract.WeaponTypes currentWeaponType;
    
    
    public WeaponAbstract GetChildedWeapon(GameObject player)
    {
        for (int i = 0; i < player.transform.childCount; i++)
        {
            Transform currentChild = player.transform.GetChild(i);
            var foundWeapon = currentChild.GetComponent<WeaponAbstract>();
            
            if (foundWeapon)
            {
                currentWeapon = foundWeapon;
                currentWeaponType = foundWeapon.weaponType;
                return currentWeapon;
            }
        }

        currentWeapon = null;
        return null;
    }

    public void AddNewWeapon(GameObject weapon)
    {
        if(currentWeapon)
            Destroy(currentWeapon);

        currentWeapon = GetChildedWeapon(this.gameObject);
        if(currentWeapon)
            Destroy(currentWeapon);

        GameObject newWeapon = PhotonNetwork.Instantiate(weapon.name, this.transform.position, Quaternion.identity);
        newWeapon.transform.parent = this.transform;
        newWeapon.transform.localScale = new Vector3(0.3f,0.3f, 0.3f);
        currentWeapon = newWeapon.GetComponent<WeaponAbstract>();
        currentWeaponType = currentWeapon.weaponType;

    }
    
}
