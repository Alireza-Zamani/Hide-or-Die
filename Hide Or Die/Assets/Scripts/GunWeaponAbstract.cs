using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(AimingDirection))]
[RequireComponent(typeof(GunPositioning))]
public class GunWeaponAbstract : WeaponAbstract
{
    private GunPositioning gunPositioning;
    private AimingDirection aimingDirection;
    
    [SerializeField] private GameObject shootingPoint;
    [SerializeField] private GameObject aimingLine;
    [SerializeField] private GameObject bulletPrefab;
    
    

    public bool isReloading = false;
    protected virtual void Awake()
    {
        gunPositioning = GetComponent<GunPositioning>();
        aimingDirection = GetComponent<AimingDirection>();
        
        weaponType = WeaponTypes.Gun;
    }

    public override void Attack()
    {
        if (isReloading)
            return;
        
        GameObject bullet =
            PhotonNetwork.Instantiate(bulletPrefab.name, shootingPoint.transform.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().Damage = Damage;
        Vector2 aimDirection = aimingDirection.AimDirection;
        aimDirection *= -1;
        
        if (transform.parent.localScale.x < 0)
            aimDirection *= -1;
        
        aimDirection = aimDirection.normalized;

        var bulletMainScript = bullet.GetComponent<Bullet>();
        
        bulletMainScript.movementDirection = aimDirection;
        bulletMainScript.ShooterTag = gameObject.tag;

        if (gameObject.CompareTag("RedTeam"))
            bulletMainScript.TargetTag = "BlueTeam";
        else if (gameObject.CompareTag("BlueTeam"))
            bulletMainScript.TargetTag = "RedTeam";
        
        aimingDirection.enabled = false;
        aimingLine.SetActive(false);
        isReloading = true;
        gunPositioning.currentState = GunPositioning.States.Reloading;
        StartCoroutine(ReloadingCounter());
    }

    public override void Aim()
    {
        if (isReloading)
            return;
        
        aimingDirection.enabled = true;
        aimingLine.SetActive(true);
        
    }


    private IEnumerator ReloadingCounter()
    {
        yield return new WaitForSeconds(6f);
        isReloading = false;
        gunPositioning.currentState = GunPositioning.States.Idle;
    }
    
}
