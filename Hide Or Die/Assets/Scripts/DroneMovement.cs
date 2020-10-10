using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DroneMovement : MovementAbstract
{

	private GameObject playerGameObject = null;

	public GameObject PlayerGameObject { get => playerGameObject; set => playerGameObject = value; }


	public override void Update()
	{
		base.Update();
	}
	
	public void SetToCharacter()
	{
		photonView.RPC("ChangeCharacter", RpcTarget.AllBuffered, playerGameObject.GetComponent<PlayerMovement>().photonView.ViewID);
		PhotonNetwork.Destroy(gameObject);
	}

	[PunRPC]
	private void ChangeCharacter(int viewID)
	{
		SetActivity(PhotonView.Find(viewID).gameObject, true);
	}

	private void SetActivity(GameObject go, bool activity)
	{
		go.SetActive(activity);
	}

}
