using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PunTeam : MonoBehaviourPunCallbacks
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

	[Header("Buttons")]
	[SerializeField] private GameObject blueTeamButton = null;
	[SerializeField] private GameObject redTeamButton = null;

	[Header("Teams Status")]
	[SerializeField] private Text blueTeamReaminer = null;
	[SerializeField] private Text redTeamReaminer = null;
	[SerializeField] private Text blueTeamStats = null;
	[SerializeField] private Text redTeamStats = null;


	[Range(0, 10)] [SerializeField] private float startMatchWaitTimeCountDown = 5f;
	private float startMatchWaitTimeCountDownTemp = 0;
	private bool matchIsStarting = false;
	private float timeCounter = 0f;


	private void Start()
	{
		maxPlayerCount = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
		startMatchWaitTimeCountDownTemp = startMatchWaitTimeCountDown;
		CheckTeamCapacity();
	}

	private void Update()
	{
		// Wait until the match is ready to start the ncount down teh timer to start the match
		if (matchIsStarting)
		{
			timeCounter += Time.deltaTime;
			if (timeCounter >= 1)
			{
				timeCounter = 0f;
				startMatchWaitTimeCountDownTemp--;
				blueTeamStats.text = "Starting The Match in : " + startMatchWaitTimeCountDownTemp.ToString();
				redTeamStats.text = "Starting The Match in : " + startMatchWaitTimeCountDownTemp.ToString();
				if (startMatchWaitTimeCountDownTemp == 0)
				{
					EnterTheGame();
					matchIsStarting = false;
					return;
				}
			}
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

	private void ScoresHashtableSetter(int teamNumber)
	{
		if(teamNumber == 1)
		{
			if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group1"))
			{
				PhotonNetwork.LocalPlayer.CustomProperties["Group1"] = 0;
			}
			else
			{
				ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Group1" , 0 }
			};

				PhotonNetwork.SetPlayerCustomProperties(playerProps);
			}
		}
		else if(teamNumber == 2)
		{
			if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group2"))
			{
				PhotonNetwork.LocalPlayer.CustomProperties["Group2"] = 0;
			}
			else
			{
				ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Group2" , 0 }
			};

				PhotonNetwork.SetPlayerCustomProperties(playerProps);
			}
		}
		
	}

	public void ChooseTeam(int teamNumber)
	{
		// Set the Hashtable for team
		TeamsHashtableSetter(teamNumber);

		// Set the Hashtable for group
		GroupsHashtableSetter(teamNumber);

		// Set the Hashtable for group score
		ScoresHashtableSetter(teamNumber);

		// Call RPC functions to call in all players because we want all players to know how many players are in the game ready to go
		TeamNum = teamNumber;
		photonView.RPC("UpdateTeams", RpcTarget.AllBuffered, teamNumber , +1);
		photonView.RPC("UpdateStats", RpcTarget.AllBuffered, teamNumber);

		// Change the remained players number
		ChangeTeamRemainer(teamNumber , true);
	}

	private void ChangeTeamRemainer(int teamNumber , bool changePanel)
	{
		if (teamNumber == 1)
		{
			if (!blueTeamPanel.gameObject.activeInHierarchy && changePanel)
			{
				SetGameObjectActivity(blueTeamPanel, true);
			}
			int remainedPlayers = maxPlayerCount - BlueTeamPlayerCount;
			blueTeamReaminer.text = remainedPlayers.ToString();
		}
		else
		{
			if (!redTeamPanel.gameObject.activeInHierarchy && changePanel)
			{
				SetGameObjectActivity(redTeamPanel, true);
			}
			int remainedPlayers = maxPlayerCount - RedTeamPlayerCount;
			redTeamReaminer.text = remainedPlayers.ToString();
		}
	}


	private void SetGameObjectActivity(GameObject panelName , bool activity)
	{
		panelName.SetActive(activity);
	}

	private void CheckTeamCapacity()
	{
		// See if the team is full set its buttons interactibility to false
		if(BlueTeamPlayerCount != maxPlayerCount)
		{
			if (!blueTeamButton.GetComponent<Button>().interactable)
			{
				blueTeamButton.GetComponent<Button>().interactable = true;
			}
		}
		if (BlueTeamPlayerCount == maxPlayerCount)
		{
			if (blueTeamButton.GetComponent<Button>().interactable)
			{
				blueTeamButton.GetComponent<Button>().interactable = false;
			}
		}

		if (RedTeamPlayerCount != maxPlayerCount)
		{
			if (!blueTeamButton.GetComponent<Button>().interactable)
			{
				redTeamButton.GetComponent<Button>().interactable = true;
			}
		}
		if (RedTeamPlayerCount == maxPlayerCount)
		{
			if (blueTeamButton.GetComponent<Button>().interactable)
			{
				redTeamButton.GetComponent<Button>().interactable = false;
			}
		}
	}


	private void EnterTheGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			SceneManager.LoadScene("MainScene");
		}
	}


	[PunRPC]
	public void UpdateTeams(int teamNumber , int changer)
	{
		if(teamNumber == 1)
		{
			BlueTeamPlayerCount += changer;
			BlueTeamPlayerCount = Mathf.Clamp(BlueTeamPlayerCount, 0, maxPlayerCount);
			CheckTeamCapacity();
		}
		else
		{
			RedTeamPlayerCount += changer;
			RedTeamPlayerCount = Mathf.Clamp(RedTeamPlayerCount, 0, maxPlayerCount);
			CheckTeamCapacity();
		}
	}

	[PunRPC]
	public void UpdateStats(int teamNumber)
	{
		if (TeamNum != 0)
		{
			// Change the remained players
			ChangeTeamRemainer(teamNumber, false);

			if (BlueTeamPlayerCount == maxPlayerCount && RedTeamPlayerCount == maxPlayerCount)
			{
				blueTeamStats.text = "Starting The Match in : " + startMatchWaitTimeCountDownTemp.ToString();
				redTeamStats.text = "Starting The Match in : " + startMatchWaitTimeCountDownTemp.ToString();
				matchIsStarting = true;
				timeCounter = 0f;
			}
			else
			{
				if (matchIsStarting == true)
				{
					matchIsStarting = false;
					startMatchWaitTimeCountDownTemp = startMatchWaitTimeCountDown;
				}
				if (BlueTeamPlayerCount != maxPlayerCount)
				{
					blueTeamStats.text = "Waiting For Players...";
				}
				else if (BlueTeamPlayerCount == maxPlayerCount)
				{
					blueTeamStats.text = "Our Team is Full Waiting For Epponents Players...";
				}
				if (RedTeamPlayerCount != maxPlayerCount)
				{
					redTeamStats.text = "Waiting For Players...";
				}
				else if (RedTeamPlayerCount == maxPlayerCount)
				{
					redTeamStats.text = "Our Team is Full Waiting For Epponents Players...";
				}
			}
		}
	}


	#region Call Backs

	public override void OnPlayerEnteredRoom(Player player)
	{
		print(player.NickName + "  Entered in   << " + PhotonNetwork.CurrentRoom.Name + " >>  And rooms player count is : " + PhotonNetwork.CurrentRoom.PlayerCount);
	}

	public override void OnPlayerLeftRoom(Player player)
	{
		// If a player leaves the game we have to change all players stats
		// If the leftef player had choosed its team this is neccesery to be done if not there is no need and we have to wait for a new player
		if (player.CustomProperties.ContainsKey("Team"))
		{
			int leftedPlayerTeam = (int)player.CustomProperties["Team"];
			UpdateTeams(leftedPlayerTeam, -1);
			UpdateStats(leftedPlayerTeam);
		}
	}

	#endregion

}
