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
		if(Team == 1)
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
	}

	private void BlueTeamFirstCondition()
	{
		droneMovement = GetComponent<PlayerMovement>().SetToDrone();
		SetShootBtnActivity(false);
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
	}

	private void SetShootBtnActivity(bool activity)
	{
		GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>().transform.GetChild(2).gameObject.SetActive(activity);
	}
}
