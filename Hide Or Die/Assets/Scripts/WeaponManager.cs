using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponManager : MonoBehaviourPunCallbacks
{
    public WeaponAbstract currentWeapon = null;
    public WeaponAbstract.WeaponTypes currentWeaponType;

    private GameObject currentWeaponObject;

    
    
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

    [PunRPC]
    private void SetParent(int instanceID)
    {
        PhotonView foundObject = PhotonView.Find(instanceID);

        if (foundObject != null)
        {
            foundObject.transform.parent = this.transform;
        }
    }
    
    public void AddNewWeapon(GameObject weapon)
    {
        if(currentWeapon)
            Destroy(currentWeapon);

        currentWeapon = GetChildedWeapon(this.gameObject);
        if(currentWeapon)
            PhotonNetwork.Destroy(currentWeapon.gameObject);
        
        GameObject newWeapon = PhotonNetwork.Instantiate(weapon.name, this.transform.position, Quaternion.identity);
        currentWeaponObject = newWeapon;
        photonView.RPC("SetParent", RpcTarget.AllBuffered, currentWeaponObject.GetComponent<PhotonView>().ViewID);
        //newWeapon.transform.parent = this.transform;
      
        currentWeapon = newWeapon.GetComponent<WeaponAbstract>();
        currentWeaponType = currentWeapon.weaponType;
        
        if (currentWeaponType == WeaponAbstract.WeaponTypes.MeleeWeapon)
            newWeapon.transform.localScale = new Vector3(0.4f,0.4f, 0.4f);
        else
            newWeapon.transform.localScale = new Vector3(-0.2f,0.2f, 0.2f);
        
    }

}
