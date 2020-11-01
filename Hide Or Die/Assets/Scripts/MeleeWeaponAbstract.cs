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
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.tag != transform.parent.tag)
        {
            if (other.tag == "BlueTeam" || other.tag == "RedTeam")
            {
                other.gameObject.GetComponent<IPlayer>().TakeDamage(Damage);

                this.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        
        
        this.GetComponent<BoxCollider2D>().enabled = false;
    }
}
