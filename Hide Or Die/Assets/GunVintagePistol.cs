using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVintagePistol : GunWeaponAbstract
{
    [SerializeField]private int gunDamage = 75;

    private void Start()
    {
        base.Damage = gunDamage;
    }
}
