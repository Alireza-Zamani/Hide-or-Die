using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class PunSpawner : MonoBehaviourPunCallbacks
{

	private GameObject player = null;

	[SerializeField] private bool isTestMode = false;

	[Header("CountDown Conditions")]
	[Range(0,60)] [SerializeField] private float countDownTimer = 30f;
	[SerializeField] private Text timerCounter = null;
	[SerializeField] private GameObject timeCounterPanel = null;

	[Header("Prefabs")]
	[SerializeField] private GameObject bluePlayerPref = null;
	[SerializeField] private GameObject redPlayerPref = null;
	[SerializeField] private GameObject objectivePrefab = null;

	[Header("Teams Spawn Points")]
	[SerializeField] private Transform blueTeamSpawnPoint = null;
	[SerializeField] private Transform redTeamSpawnPoint = null;

	[Header("Objectives Spawn Points")]
	[SerializeField] private Transform objectivesSpawnPoint = null;

	private bool timerStarted = false;
	private float counter = 0f;
	private PlayerEnterGameDuty playerEnterGameDuty = null;

	private void Start()
	{
		if (isTestMode)
		{
			return;
		}
		SpawnHeros();
	}

	private void Update()
	{
		if (timerStarted)
		{
			counter += Time.deltaTime;
			if(counter >= 1)
			{
				counter = 0;
				countDownTimer -= 1;
				timerCounter.text = countDownTimer.ToString();
				if(countDownTimer == 0)
				{
					playerEnterGameDuty.PlayerEnteredFinished();
					timeCounterPanel.SetActive(false);
					timerStarted = false;
				}
			}
		}
	}

	private void SpawnHeros()
	{
		if (PhotonNetwork.IsConnected)
		{
			// Get the team and spawn related to that
			if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
			{
				Debug.LogError("No hashTable exists for team");
			}

			int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

			// Team Blue
			if(team == 1)
			{
				player = PhotonNetwork.Instantiate(bluePlayerPref.name, blueTeamSpawnPoint.position , Quaternion.identity);
			}
			// Team Red
			else if(team == 2)
			{
				player = PhotonNetwork.Instantiate(redPlayerPref.name, redTeamSpawnPoint.position, Quaternion.identity);
			}


			if (PhotonNetwork.IsMasterClient)
			{
				SpawnObjectives();
			}

			// Set the first condition for the player
			playerEnterGameDuty = player.GetComponent<PlayerEnterGameDuty>();
			playerEnterGameDuty.Team = team;
			playerEnterGameDuty.PlayerEntered();
			StartTimerCountDown();
		}
	}

	private void StartTimerCountDown()
	{
		timeCounterPanel.SetActive(true);
		timerStarted = true;
	}

	private void SpawnObjectives()
	{
		PhotonNetwork.Instantiate(objectivePrefab.name, objectivesSpawnPoint.position, Quaternion.identity);
	}
}
