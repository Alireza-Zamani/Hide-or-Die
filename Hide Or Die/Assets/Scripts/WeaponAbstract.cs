using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviourPunCallbacks
{
    private int damage = 30;
    
    public enum WeaponTypes { MeleeWeapon, Gun }

    public WeaponTypes weaponType;
    

    public int Damage {get => damage; set => damage = value;}
    public WeaponTypes WeaponType { get => weaponType; set => weaponType = value;}


    public virtual void Attack() {}
    public virtual void Aim() {}
    
    
}
