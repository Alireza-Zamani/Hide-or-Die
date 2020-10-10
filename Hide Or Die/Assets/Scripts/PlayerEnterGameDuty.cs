using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerEnterGameDuty : MonoBehaviourPunCallbacks
{

	private int team = 0;
	public int Team { get => team; set => team = value; }


	private GameObject shootingBtn = null;


	private DroneMovement droneMovement = null;

	private void Start()
	{

	}

	public void PlayerEntered()
	{
		// Get the team and spawn related to that
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
		{
			Debug.LogError("No hashTable exists for team");
		}

		int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
		photonView.RPC("SetLayerAndTag", RpcTarget.AllBuffered, team);


		if (Team == 1)
		{
			// Blue Team ===> the player will change to a drone to spectacle the map
			BlueTeamFirstCondition();
		}
		else if(Team == 2)
		{
			// Blue Team ===> the player will have time to set their positions
			RedTeamFirstCondition();
		}
	}

	[PunRPC]
	private void SetLayerAndTag(int team)
	{

		string tagName = null;
		if (team == 1)
		{
			tagName = "BlueTeam";
		}
		else if (team == 2)
		{
			tagName = "RedTeam";
		}

		gameObject.tag = tagName;
	}

	private void RedTeamFirstCondition()
	{
		SetShootBtnActivity(false);
	}

	private void BlueTeamFirstCondition()
	{
		droneMovement = GetComponent<PlayerMovement>().SetToDrone();
		SetShootBtnActivity(false);
		SetShopBtnActivity(false);
	}

	public void PlayerEnteredFinished()
	{
		if (Team == 1)
		{
			// Blue Team ===> the player will change to a character
			BlueTeamSecondCondition();
		}
		else if (Team == 2)
		{
			// Blue Team ===> the player will have time to set their positions
			RedTeamSecondCondition();
		}
	}

	private void RedTeamSecondCondition()
	{
		SetShootBtnActivity(true);
	}

	private void BlueTeamSecondCondition()
	{
		droneMovement.SetToCharacter();
		SetShootBtnActivity(true);
		SetShopBtnActivity(true);
	}


	private void SetShootBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(2).gameObject.SetActive(activity);
	}

	private void SetShopBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(3).gameObject.SetActive(activity);
	}
}
