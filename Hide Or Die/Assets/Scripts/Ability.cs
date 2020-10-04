using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ability : AbilityAbstract
{

	[SerializeField] private GameObject grenadePrefab = null;
	[Range(0, 1000)] [SerializeField] private float throwSpeed = 200f;
	private GameObject newGrenade = null;

	public override void ThrowGrenade(Vector2 aimingDirection)
	{
		newGrenade = PhotonNetwork.Instantiate(grenadePrefab.name, transform.position, Quaternion.identity);
		aimingDirection = aimingDirection.normalized * throwSpeed;
		newGrenade.GetComponent<Rigidbody2D>().AddForce(aimingDirection, ForceMode2D.Force);
		print(aimingDirection);
	}

}
