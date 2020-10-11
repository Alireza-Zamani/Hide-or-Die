using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityGrenade : AbilityAbstract
{

	[SerializeField] private GameObject grenadePrefab = null;
	
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
		Grenade grenade = newGrenade.GetComponent<Grenade>();
		grenade.PlayerInterface = playerInterface;
		aimingDirection = aimingDirection.normalized;
		grenade.AimingDirection = aimingDirection;
	}

}
