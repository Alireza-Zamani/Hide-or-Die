using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponSaber : MeleeWeaponAbstract
{
    public int weaponDamage = 30;
    
    private void Awake()
    {
        base.Damage = weaponDamage;
    }
}
