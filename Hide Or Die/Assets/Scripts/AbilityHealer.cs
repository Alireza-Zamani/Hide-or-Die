﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityHealer : AbilityAbstract
{
	[SerializeField] private GameObject healPrefab = null;

	private GameObject newHealer = null;

	private IPlayer playerInterface = null;

	private void Awake()
	{
		healPrefab = Resources.Load("Heal", typeof(GameObject)) as GameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void ExecuteAbility(Vector2 aimingDirection)
	{
		newHealer = PhotonNetwork.Instantiate(healPrefab.name, transform.position, Quaternion.identity);
		newHealer.GetComponent<Heal>().PlayerInterface = playerInterface;
	}
	
}
