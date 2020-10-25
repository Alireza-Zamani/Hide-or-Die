using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 30;
    [SerializeField] private float bulletSpeed = 100;
    public Vector3 movementDirection;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Other"))
        {
            other.gameObject.GetComponent<PlayerMatchData>().TakeDamage(this.Damage);
        }
        
        Destroy(this.gameObject);
    }
}
