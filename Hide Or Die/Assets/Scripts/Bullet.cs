using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    private int damage = 70;
    [SerializeField] private float bulletSpeed = 100;
    public Vector3 movementDirection;

    private string targetTag;
    private string shooterTag;

    private void Awake()
    {
        Destroy(this.gameObject, 5f);
    }

    private void Update()
    {
        transform.Translate(movementDirection * bulletSpeed * Time.deltaTime);
    }

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public string ShooterTag
    {
        get => shooterTag;
        set => shooterTag = value;
    }

    public string TargetTag
    {
        get => targetTag;
        set => targetTag = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.tag != shooterTag)
        {
            if (other.tag == "BlueTeam" || other.tag == "RedTeam")
            {
                other.GetComponent<PlayerMatchData>().TakeDamage(Damage);
            }

            Destroy(this.gameObject);
        }
    }
}
