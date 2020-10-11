﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Heal : MonoBehaviour
{
	[Range(0, 10)] [SerializeField] private float healEndTimeCounter = 5f;
	[Range(0, 10)] [SerializeField] private float timeEffectRate = 1f;
	[Range(0, 100)] [SerializeField] private float healEffectAmount = 50f;
	[Range(0, 10)] [SerializeField] private float radiousOfAction = 5f;
	[SerializeField] private LayerMask raycastableForInSightLayerMask = new LayerMask();

	[SerializeField] private LayerMask healableLayerMask = new LayerMask();

	private float timeCounter = 0f;

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	private PhotonView photonView;

	private string team = null;

	private void Start()
	{
		photonView = PhotonView.Get(this);
		if (!photonView.IsMine)
		{
			Destroy(this);
			return;
		}

		team = playerInterface.TeamGetter();
		Invoke("DestroyGameObject", timeEffectRate);
	}

	private void Update()
	{
		timeCounter += Time.deltaTime;
		if(timeCounter >= timeEffectRate)
		{
			timeCounter = 0f;
			Healing();
		}
	}

	private void Healing()
	{
		// Finds all players in the radious
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, healableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			// Finds which one of these players are in the sight
			Vector2 dir = coll.collider.gameObject.transform.position - transform.position;
			float distance = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
			RaycastHit2D hittest = Physics2D.Raycast(transform.position, dir.normalized, distance, raycastableForInSightLayerMask);
			if (hittest.collider == null)
			{
				if (coll.collider.gameObject.GetComponent<IPlayer>() != null && coll.collider.gameObject.tag == team)
				{
					playerInterface = coll.collider.gameObject.GetComponent<IPlayer>();
					playerInterface.Heal(healEffectAmount);
					print("Is Healing");
				}
			}
		}		
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
