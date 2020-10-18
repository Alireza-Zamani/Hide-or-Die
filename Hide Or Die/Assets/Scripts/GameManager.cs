﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Configuration;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{

	[SerializeField] private int winNeededScoreLimit = 2;
	private bool hasGameEnded = false;

	private int team = 0;
	private int group = 0;
	public int Group { get => group; set => group = value; }

	private int groupScore = 0;
	public int GroupScore
	{
		get => groupScore;
		set
		{
			groupScore = value;
			if (groupScore == winNeededScoreLimit)
			{
				if(Group == 1)
				{
					hasGameEnded = true;
					photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient);
					print("Group 1 won the game!!!");
				}
				else if(Group == 2)
				{
					hasGameEnded = true;
					photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient);
					print("Group 2 won the game!!!");
				}
			}
		}
	}




	private int teamGroup1RemainedPlayers = 0;
    public int TeamGroup1RemainedPlayers
	{
		get => teamGroup1RemainedPlayers;
		set
		{ 
			teamGroup1RemainedPlayers = value;
			if (teamGroup1RemainedPlayers == 0)
			{
				SetTheGroupScore(2);
				TheNextMatch();
				print("Group 2 won the match!!!");
			}
		} 
	}


	private int teamGroup2RemainedPlayers = 0;
	public int TeamGroup2RemainedPlayers
	{
		get => teamGroup2RemainedPlayers;
		set
		{
			teamGroup2RemainedPlayers = value;
			if (teamGroup2RemainedPlayers == 0)
			{
				SetTheGroupScore(1);
				TheNextMatch();
				print("Group 1 won the match!!!");
			}
		}
	}

	private void Start()
	{
		// Set the group number
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			Debug.LogError("No group hash added");
		}
		else
		{
			Group = (int)PhotonNetwork.LocalPlayer.CustomProperties["Group"];
		}

		// Get the team because we will use it in the loosing to say that the opponent teams score should increase and to see which group its in
		team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
		
		// Get the teams remained players count
		int allPlayersCount = PhotonNetwork.CurrentRoom.PlayerCount;
		TeamGroup1RemainedPlayers = TeamGroup2RemainedPlayers = allPlayersCount / 2;

		// Get the teams scores
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
		{
			Debug.LogError("No team hash added");
		}
		else
		{
			if(team != 0)
			{
				GetTheGroupScore();
			}
		}
	}


	private void TheNextMatch()
	{
		ChangeTheTeamsRoleForTheNextMatch();
		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("CallTheNextMatch", 3f);
		}
	}


	private void CallTheNextMatch()
	{
		if (hasGameEnded)
		{

			PhotonNetwork.LoadLevel("Pun");
		}
		else
		{
			PhotonNetwork.LoadLevel("NextMatch");
		}
	}


	private void GetTheGroupScore()
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			Debug.LogError("No group hash added");
		}
		else
		{
			if (Group == 1)
			{
				GroupScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["Group1"];
			}
			else if (Group == 2)
			{
				GroupScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["Group2"];
			}
		}
	}

	private void SetTheGroupScore(int winnerGroup)
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			Debug.LogError("No group hash added");
		}
		else
		{
			if (winnerGroup == Group)
			{
				GroupScore++;
				if (Group == 1)
				{
					PhotonNetwork.LocalPlayer.CustomProperties["Group1"] = GroupScore;
				}
				else if (Group == 2)
				{
					PhotonNetwork.LocalPlayer.CustomProperties["Group2"] = GroupScore;
				}
			}
		}
	}

	[PunRPC]
	private void RPCGameHasFinished()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("FinishGame", 2f);
		}
	}

	private void FinishGame()
	{
		PhotonNetwork.LoadLevel("Pun");
	}


	private void ChangeTheTeamsRoleForTheNextMatch()
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
		{
			Debug.LogError("No team hash added");
		}
		else
		{
			int newTeam = 0;
			if(team == 1)
			{
				newTeam = 2;
			}
			else if(team == 2)
			{
				newTeam = 1;
			}
			PhotonNetwork.LocalPlayer.CustomProperties["Team"] = newTeam;
		}
	}


}