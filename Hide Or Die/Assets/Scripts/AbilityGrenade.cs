using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityGrenade : AbilityAbstract
{

	[SerializeField] private GameObject grenadePrefab = null;
	private float throwSpeed = 800f;
	private GameObject newGrenade = null;

	private IPlayer playerInterface = null;

	private void Awake()
	{
		grenadePrefab = Resources.Load("Grenade", typeof(GameObject)) as GameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void ExecuteAbility(Vector2 aimingDirection)
	{
		newGrenade = PhotonNetwork.Instantiate(grenadePrefab.name, transform.position, Quaternion.identity);
		newGrenade.GetComponent<Grenade>().PlayerInterface = playerInterface;
		aimingDirection = aimingDirection.normalized * throwSpeed;
		newGrenade.GetComponent<Rigidbody2D>().AddForce(aimingDirection, ForceMode2D.Force);
	}

}
