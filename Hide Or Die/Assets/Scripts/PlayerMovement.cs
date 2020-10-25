using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MovementAbstract
{

	[SerializeField] private GameObject dronePrefab = null;

	private bool isStucked = false;
	public bool IsStucked { get => isStucked; set => photonView.RPC("RPCSetisStucked", RpcTarget.AllBuffered, value); }

	public override void Update()
	{
		if (IsStucked)
		{
			return;
		}
		base.Update();
	}

	public DroneMovement SetToDrone()
	{
		// Create new drone
		GameObject newDrone = null;
		newDrone = PhotonNetwork.Instantiate(dronePrefab.name, transform.position, Quaternion.identity);
		DroneMovement droneMovement = null;
		droneMovement = newDrone.GetComponent<DroneMovement>();
		droneMovement.PlayerGameObject = gameObject;

		// Tell every one to set player game object off (change character)
		photonView.RPC("ChangeCharacter", RpcTarget.AllBuffered);

		return droneMovement;
	}


	private void SetActivity(GameObject go , bool activity)
	{
		go.SetActive(activity);
	}


	[PunRPC]
	private void ChangeCharacter()
	{

		SetActivity(gameObject, false);
	}


	[PunRPC]
	private void RPCSetisStucked(bool value)
	{
		isStucked = value;
	}
}
