using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PunSpawner : MonoBehaviourPunCallbacks
{

	private GameObject player = null;

	[SerializeField] private bool isTestMode = false;

	[Header("Prefabs")]
	[SerializeField] private GameObject bluePlayerPref = null;
	[SerializeField] private GameObject redPlayerPref = null;

	[Header("Spawn Points")]
	[SerializeField] private Transform blueTeamSpawnPoint = null;
	[SerializeField] private Transform redTeamSpawnPoint = null;

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
				print("Error");
			}
				int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

			if(team == 1)
			{
				player = PhotonNetwork.Instantiate(bluePlayerPref.name, blueTeamSpawnPoint.position , Quaternion.identity);
			}
			else if(team == 2)
			{
				player = PhotonNetwork.Instantiate(redPlayerPref.name, redTeamSpawnPoint.position, Quaternion.identity);
			}			
		}
	}

}
