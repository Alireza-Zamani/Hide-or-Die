using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MovementAbstract
{

	[SerializeField] private GameObject dronePrefab = null;

	public override void Update()
	{
		base.Update();
		//if (photonView.IsMine)
		//{
		//	if (Input.GetKeyDown(KeyCode.Space))
		//	{
		//		SetToDrone();
		//	}
		//}
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

	[PunRPC]
	private void ChangeCharacter()
	{

		SetActivity(gameObject, false);
	}

	private void SetActivity(GameObject go , bool activity)
	{
		go.SetActive(activity);
	}

	
}
