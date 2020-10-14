using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    private int damage = 30;
    
    public enum WeaponTypes { MeleeWeapon, Gun }

    public WeaponTypes weaponType;
    

    public int Damage {get => damage; set => damage = value;}
    public WeaponTypes WeaponType { get => weaponType; set => weaponType = value;}


    public virtual void Attack() {}
    public virtual void Aim() {}
    
    
}
