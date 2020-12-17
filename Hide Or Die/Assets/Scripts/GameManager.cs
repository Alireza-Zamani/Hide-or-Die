using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Configuration;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{

	[Range(0, 300)] [SerializeField] private float countDownTimerForRoundFinish = 90f;
	private float timeCounter = 0f;

	[SerializeField] private Text blueTeamScoreText = null;
	[SerializeField] private Text redTeamScoreText = null;
	[SerializeField] private Text roundText = null;
	[SerializeField] private Text roundTimerText = null;

	[SerializeField] private GameObject losePanel = null;
	[SerializeField] private GameObject winPanel = null;
	[SerializeField] private int winNeededScoreLimit = 2;
	private bool hasGameEnded = false;
	private bool roundTimeFinished = false;

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
					photonView.RPC("SetTheWinLosePanel", RpcTarget.AllBuffered, 1);
					photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient , 1);
					print("Group 1 won the game!!!");
				}
				else if(Group == 2)
				{
					hasGameEnded = true;
					photonView.RPC("SetTheWinLosePanel", RpcTarget.AllBuffered, 2);
					photonView.RPC("RPCGameHasFinished", RpcTarget.MasterClient , 2);
					print("Group 2 won the game!!!");
				}
			}
		}
	}


	private int group1Score = -1;
	private int group2Score = -1;

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
				TheNextMatch(2);
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
				TheNextMatch(1);
				print("Group 1 won the match!!!");
			}
		}
	}

	private void Start()
	{
		if (countDownTimerForRoundFinish >= 60)
		{
			float iFloat = countDownTimerForRoundFinish / 60;
			int iInt = (int) iFloat;
			float secondFloat = (countDownTimerForRoundFinish - (iInt * 60));
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "0" + iInt + " : " + secondString;
		}
		else if (countDownTimerForRoundFinish > 0)
		{
			float secondFloat = countDownTimerForRoundFinish;
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "00" + " : " + secondString;
		}

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
		int allPlayersCount = PhotonNetwork.CurrentRoom.MaxPlayers;
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
				photonView.RPC("SendGroupScoreToMaster", RpcTarget.MasterClient, GroupScore , Group);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("SetTheMatchScores", 2f);
		}
	}

	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (roundTimeFinished)
			{
				return;
			}
			timeCounter += Time.deltaTime;
			if(timeCounter >= 1)
			{
				countDownTimerForRoundFinish--;
				timeCounter = 0f;
				photonView.RPC("RPCChangeTheTimeOfRoundText", RpcTarget.AllBuffered, countDownTimerForRoundFinish);
				if(countDownTimerForRoundFinish <= 0)
				{
					if (!hasGameEnded)
					{
						
						roundTimeFinished = true;
						photonView.RPC("RoundTimeFinished", RpcTarget.AllBuffered);
					}
				}
			}
		}
	}

	private void SetTheMatchScores()
	{
		if(group1Score == -1 || group2Score == -1)
		{
			Invoke("SetTheMatchScores", 2f);
			return;
		}
		photonView.RPC("RPCSetTheMatchScores", RpcTarget.AllBuffered, group1Score, group2Score);
	}

	private void TheNextMatch(int winnerTeam)
	{
		ChangeTheTeamsRoleForTheNextMatch();
		if (PhotonNetwork.IsMasterClient)
		{
			float waitTime = 3f;
			Invoke("CallTheNextMatch", waitTime);
		}
	}


	private void CallTheNextMatch()
	{
		if (!hasGameEnded)
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
	private void RPCChangeTheTimeOfRoundText(float remainedTime)
	{
		if (remainedTime >= 60)
		{
			float iFloat = remainedTime / 60;
			int iInt = (int)iFloat;
			float secondFloat = (remainedTime - (iInt * 60));
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "0" + iInt + " : " + secondString;
		}
		else if (remainedTime > 0)
		{
			float secondFloat = remainedTime;
			string secondString = secondFloat.ToString();
			if (secondFloat < 10)
			{
				secondString = "0" + secondString;
			}
			roundTimerText.text = "00" + " : " + secondString;
		}
		//if (remainedTime >= 120)
		//{
		//	roundTimerText.text = "02 : " + (remainedTime - 120).ToString();
		//}
		//else if (remainedTime >= 60)
		//{
		//	roundTimerText.text = "01 : " + (remainedTime - 60).ToString();
		//}
		//else if(remainedTime >= 0)
		//{
		//	roundTimerText.text = "00 : " + (remainedTime).ToString();
		//}

	}

	[PunRPC]
	private void RoundTimeFinished()
	{
		if(team == 2)
		{
			return;
		}
		if (Group == 1)
		{
			photonView.RPC("RPCTeamGroup1RemainedPlayers", RpcTarget.AllBuffered);
		}
		else if (Group == 2)
		{
			photonView.RPC("RPCTeamGroup2RemainedPlayers", RpcTarget.AllBuffered);
		}
	}

	[PunRPC]
	private void RPCTeamGroup1RemainedPlayers()
	{
		TeamGroup1RemainedPlayers--;
	}

	[PunRPC]
	private void RPCTeamGroup2RemainedPlayers()
	{
		TeamGroup2RemainedPlayers--;
	}

	[PunRPC]
	private void RPCSetTheMatchScores(int group1Score , int group2Score)
	{
		roundText.text = (group1Score + group2Score + 1).ToString();
		if(Group == 1)
		{
			if(team == 1)
			{
				blueTeamScoreText.text = group1Score.ToString();
				redTeamScoreText.text = group2Score.ToString();
			}
			else if(team == 2)
			{
				blueTeamScoreText.text = group2Score.ToString();
				redTeamScoreText.text = group1Score.ToString();
			}
		}
		else if(Group == 2)
		{
			if (team == 1)
			{
				blueTeamScoreText.text = group2Score.ToString();
				redTeamScoreText.text = group1Score.ToString();
			}
			else if (team == 2)
			{
				blueTeamScoreText.text = group1Score.ToString();
				redTeamScoreText.text = group2Score.ToString();
			}
		}
	}

	[PunRPC]
	private void SendGroupScoreToMaster(int groupScore , int group)
	{
		if(group == 1)
		{
			if(group1Score == -1)
			{
				group1Score = groupScore;
			}
		}
		else if(group == 2)
		{
			if (group2Score == -1)
			{
				group2Score = groupScore;
			}
		}
	}


	[PunRPC]
	private void SetTheWinLosePanel(int winnerGroup)
	{
		if(Group == winnerGroup)
		{
			winPanel.SetActive(true);
		}
		else
		{
			losePanel.SetActive(true);
		}
	}

	[PunRPC]
	private void RPCGameHasFinished(int winnerGroup)
	{
		hasGameEnded = true;
		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("FinishGame", 5f);
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
