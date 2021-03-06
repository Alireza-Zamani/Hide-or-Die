﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityRevive : AbilityAbstract
{
	[SerializeField] private GameObject revivePrefab = null;

	private GameObject newRevive = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private GameObject fixedJoyStick = null;

	private void Awake()
	{
		revivePrefab = Resources.Load("Revive", typeof(GameObject)) as GameObject;
		fixedJoyStick = GameObject.FindGameObjectWithTag("UI").transform.GetChild(8).gameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}


	public override void AbilityIsStarting(GameObject aimingPref)
	{
		if (fixedJoyStick.activeInHierarchy)
		{
			fixedJoyStick.SetActive(false);
		}
	}

	public override void ExecuteAbility()
	{
		if (newAiming != null)
		{
			Destroy(newAiming);
		}

		newRevive = PhotonNetwork.Instantiate(revivePrefab.name, transform.position, Quaternion.identity);
		newRevive.GetComponent<Revive>().PlayerInterface = playerInterface;
	}
}
