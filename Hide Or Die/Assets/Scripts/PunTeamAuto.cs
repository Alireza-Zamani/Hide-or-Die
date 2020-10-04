﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PunTeamAuto : MonoBehaviourPunCallbacks
{
	private int teamNum = 0;
	public int TeamNum { get => teamNum; set => teamNum = value; }

	private int blueTeamPlayerCount = 0;
	public int BlueTeamPlayerCount { get => blueTeamPlayerCount; set => blueTeamPlayerCount = value; }

	private int redTeamPlayerCount = 0;
	public int RedTeamPlayerCount { get => redTeamPlayerCount; set => redTeamPlayerCount = value; }

	private int maxPlayerCount = 1;

	[Header("Panels")]
	[SerializeField] private GameObject blueTeamPanel = null;
	[SerializeField] private GameObject redTeamPanel = null;

	[Header("Teams Status")]
	[SerializeField] private Text blueTeamReaminer = null;
	[SerializeField] private Text redTeamReaminer = null;
	[SerializeField] private Text blueTeamStats = null;
	[SerializeField] private Text redTeamStats = null;



	private void Start()
	{
		maxPlayerCount = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
		Invoke("Choose", 1f);
	}

	private void Choose()
	{
		ChooseTeamAuto();
	}

	private void ChooseTeamAuto()
	{
		// See if blue team is empty full it otherwise full the red team
		if (BlueTeamPlayerCount != maxPlayerCount)
		{
			ChooseTeam(1);
		}
		else
		{
			ChooseTeam(2);
		}
	}

	private void TeamsHashtableSetter(int teamNumber)
	{
		if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
		{
			PhotonNetwork.LocalPlayer.CustomProperties["Team"] = teamNumber;
		}
		else
		{
			ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Team" , teamNumber }
			};

			PhotonNetwork.SetPlayerCustomProperties(playerProps);
		}
	}

	private void GroupsHashtableSetter(int teamNumber)
	{
		if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			PhotonNetwork.LocalPlayer.CustomProperties["Group"] = teamNumber;
		}
		else
		{
			ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Group" , teamNumber }
			};

			PhotonNetwork.SetPlayerCustomProperties(playerProps);
		}
	}

	public void ChooseTeam(int teamNumber)
	{
		// Set the Hashtable for team
		TeamsHashtableSetter(teamNumber);

		// Set the Hashtable for group
		GroupsHashtableSetter(teamNumber);


		// Call RPC functions to call in all players
		TeamNum = teamNumber;

		photonView.RPC("UpdateTeams", RpcTarget.AllBuffered, teamNumber);
		photonView.RPC("UpdateStats", RpcTarget.AllBuffered, teamNumber);

		if (teamNumber == 1)
		{
			SetGameObjectActivity(blueTeamPanel, true);
			int remainedPlayers = maxPlayerCount - BlueTeamPlayerCount;
			blueTeamReaminer.text = remainedPlayers.ToString();
		}
		else
		{
			SetGameObjectActivity(redTeamPanel, true);
			int remainedPlayers = maxPlayerCount - RedTeamPlayerCount;
			redTeamReaminer.text = remainedPlayers.ToString();
		}
	}

	[PunRPC]
	public void UpdateTeams(int teamNumber)
	{
		if (teamNumber == 1)
		{
			BlueTeamPlayerCount++;
		}
		else
		{
			RedTeamPlayerCount++;
		}
	}

	[PunRPC]
	public void UpdateStats(int teamNumber)
	{
		if (TeamNum != 0)
		{
			if (BlueTeamPlayerCount == maxPlayerCount && RedTeamPlayerCount == maxPlayerCount)
			{
				blueTeamStats.text = "Starting The Match...";
				redTeamStats.text = "Starting The Match...";

				Invoke("EnterTheGame", 1f);
			}
			else if (BlueTeamPlayerCount == maxPlayerCount)
			{
				blueTeamStats.text = "Our Team is Full Waiting For Epponents Players...";
			}
			else if (RedTeamPlayerCount == maxPlayerCount)
			{
				redTeamStats.text = "Our Team is Full Waiting For Epponents Players...";
			}
		}
	}

	private void SetGameObjectActivity(GameObject panelName, bool activity)
	{
		panelName.SetActive(activity);
	}

	private void EnterTheGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			SceneManager.LoadScene("MainScene");
		}
	}


	#region Call Backs

	public override void OnPlayerEnteredRoom(Player player)
	{
		print(player.NickName + "  Entered in   << " + PhotonNetwork.CurrentRoom.Name + " >>  And rooms player count is : " + PhotonNetwork.CurrentRoom.PlayerCount);
	}


	#endregion


}