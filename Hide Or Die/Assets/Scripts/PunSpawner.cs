using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PunSpawner : MonoBehaviourPunCallbacks
{

	private GameObject player = null;

	[SerializeField] private bool isTestMode = false;

	[Header("Prefabs")]
	[SerializeField] private GameObject bluePlayerPref = null;
	[SerializeField] private GameObject redPlayerPref = null;
	[SerializeField] private GameObject objectivePrefab = null;


	[Header("Teams Spawn Points")]
	[SerializeField] private Transform blueTeamSpawnPoint = null;
	[SerializeField] private Transform redTeamSpawnPoint = null;

	[Header("Objectives Spawn Points")]
	[SerializeField] private Transform objectivesSpawnPoint = null;


	private void Start()
	{
		if (isTestMode)
		{
			return;
		}
		SpawnHeros();
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
		}
	}

	private void SpawnObjectives()
	{
		PhotonNetwork.Instantiate(objectivePrefab.name, objectivesSpawnPoint.position, Quaternion.identity);
	}
}
