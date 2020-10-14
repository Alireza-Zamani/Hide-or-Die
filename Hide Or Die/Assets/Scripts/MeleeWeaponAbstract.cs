using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeleeWeaponTargeting))]
[RequireComponent(typeof(MeleeWeaponPositioning))]
public class MeleeWeaponAbstract : WeaponAbstract
{
    private MeleeWeaponPositioning meleeWeaponPositioning = null;

    protected virtual void Awake()
    {
        weaponType = WeaponTypes.MeleeWeapon;
    }

    public override void Attack()
    {
        if (!meleeWeaponPositioning)
        {
            meleeWeaponPositioning = GetComponent<MeleeWeaponPositioning>();
            
            if(!meleeWeaponPositioning)
                return;
        }

        if (meleeWeaponPositioning.currentState == MeleeWeaponPositioning.States.Attacking)
            return;
            
        
        meleeWeaponPositioning.currentState = MeleeWeaponPositioning.States.Attacking;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Other"))
        {
            other.gameObject.GetComponent<PlayerMatchData>().TakeDamage(this.Damage);
        }
    }
}
