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

	[Header("Colors OF Teams")]
	[SerializeField] private Color blueTeamColor = Color.white;
	[SerializeField] private Color redTeamColor = Color.white;


	public void PlayerEntered()
	{
		photonView.RPC("SetLAyerAndTag", RpcTarget.AllBuffered, Team);

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


	private void RedTeamFirstCondition()
	{
		SetShootBtnActivity(false);
		SetMineBtnActivity(false);
	}

	private void BlueTeamFirstCondition()
	{
		droneMovement = GetComponent<PlayerMovement>().SetToDrone();
		SetShootBtnActivity(false);
		SetShopBtnActivity(false);
		SetMineBtnActivity(false);
		SetWeaponBtnActivity(false);
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
		SetWeaponBtnActivity(true);
	}


	private void SetShootBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(2).gameObject.SetActive(activity);
	}

	private void SetShopBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(3).gameObject.SetActive(activity);
	}
	
	private void SetWeaponBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(7).gameObject.SetActive(activity);
	}


	private void SetMineBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(6).gameObject.SetActive(activity);
	}


	[PunRPC]
	private void SetLAyerAndTag(int team)
	{
		string tagName = null;
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		if (team == 1)
		{
			tagName = "BlueTeam";
			//sprite.color = blueTeamColor;
			gameObject.name = "Blue Player";
		}
		else if (team == 2)
		{
			tagName = "RedTeam";
			//sprite.color = redTeamColor;
			gameObject.name = "Red Player";
		}
		gameObject.tag = tagName;

		// Set the players color
	}
}
