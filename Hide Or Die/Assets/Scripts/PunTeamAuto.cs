using System.Collections;
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

	[Header("Players Status")]
	[SerializeField] private GameObject bluePlayersContainer = null;
	[SerializeField] private GameObject redPlayersContainer = null;
	[SerializeField] private GameObject players = null;

	[Header("Players Status")]
	[Range(0, 2)] [SerializeField] private float autoClassSelectWaitForMasterCheck = 1f;
	[Range(0, 2)] [SerializeField] private float autoClassSelectWaitForSecondCurrentSelection= 1f;


	[Range(0, 10)] [SerializeField] private float startMatchWaitTimeCountDown = 5f;
	private float startMatchWaitTimeCountDownTemp = 0;
	private bool matchIsStarting = false;
	private float timeCounter = 0f;
	private bool canForceSelectAbility = false;


	private PUNUIBtns punUiBtns = null;

	//Master Client
	private bool masterClientForceSelectAbility = false;
	private bool masterClientForceSelectAbilityStarted = false;
	private float forceSelectAbilityCoolDown = 3f;
	private float forceSelectAbilityTimer = 0f;
	private List<string> notChoosedAbilityPlayersUserIds = new List<string>();

	private void Start()
	{
		punUiBtns = GameObject.FindGameObjectWithTag("UI").GetComponent<PUNUIBtns>();
		maxPlayerCount = PhotonNetwork.CurrentRoom.MaxPlayers / 2;
		startMatchWaitTimeCountDownTemp = startMatchWaitTimeCountDown;
		Invoke("Choose", 1f);
	}

	private void Update()
	{
		if (masterClientForceSelectAbility && !masterClientForceSelectAbilityStarted)
		{
			masterClientForceSelectAbilityStarted = true;
			Invoke("SetARandomeAbilityForClients", autoClassSelectWaitForMasterCheck);
			masterClientForceSelectAbility = false;
		}
		// Wait until the match is ready to start the ncount down teh timer to start the match
		if (matchIsStarting)
		{
			// If match is starting and the player has not selected his ability then match will randomly select a remained ability for him
			if (canForceSelectAbility && !punUiBtns.ChoosedAbility)
			{
				// Send the players user id to master because master will choose a randome class for it
				photonView.RPC("RPCSendUserIdToMasterClient", RpcTarget.MasterClient, (string)PhotonNetwork.LocalPlayer.UserId);

				// Say to master that u have to start the randome choose function
				photonView.RPC("RPCMasterClientForceSelectAbility", RpcTarget.MasterClient, true);

				canForceSelectAbility = false;
			}

			timeCounter += Time.deltaTime;
			if(timeCounter >= 1)
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

	private void SetARandomeAbilityForClients()
	{
		StartCoroutine("StartRandomeAbilityCoroutine");
	}

	IEnumerator StartRandomeAbilityCoroutine()
	{
		if (notChoosedAbilityPlayersUserIds.Count == 0)
		{
			yield return null;
		}
		else
		{
			string userId = notChoosedAbilityPlayersUserIds[0];
			notChoosedAbilityPlayersUserIds.RemoveAt(0);
			photonView.RPC("CheckIfThePlayerCanChooseRandomeAbility", RpcTarget.AllBuffered, userId);
		}
		yield return new WaitForSeconds(autoClassSelectWaitForSecondCurrentSelection);
		StartCoroutine("StartRandomeAbilityCoroutine");
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

	private void ScoresHashtableSetter(int teamNumber)
	{
		if (teamNumber == 1)
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
		else if (teamNumber == 2)
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

		// Call RPC functions to call in all players
		TeamNum = teamNumber;

		photonView.RPC("UpdateTeams", RpcTarget.AllBuffered, teamNumber , 1);
		photonView.RPC("UpdateStats", RpcTarget.AllBuffered, teamNumber);
		ChangeTeamRemainer(teamNumber , true);

		// Change Player Containers
		photonView.RPC("RPCPlayerContainer", RpcTarget.AllBuffered, teamNumber, PhotonNetwork.LocalPlayer.NickName , PhotonNetwork.LocalPlayer.UserId);
	}

	private void ChangeTeamRemainer(int teamNumber, bool changePanel)
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


	#region RPCs


	[PunRPC]
	private void CheckIfThePlayerCanChooseRandomeAbility(string userId)
	{
		// If we are the user which the randome ability selections turn is its then choose one
		if (PhotonNetwork.LocalPlayer.UserId == userId)
		{
			punUiBtns.AutoSelectAbility();
		}
	}

	[PunRPC]
	private void RPCSendUserIdToMasterClient(string userId)
	{
		notChoosedAbilityPlayersUserIds.Add(userId);
	}

	[PunRPC]
	private void RPCMasterClientForceSelectAbility(bool activity)
	{
		// Say to master that we have players with non ability choosed
		if (masterClientForceSelectAbility != activity)
		{
			masterClientForceSelectAbility = activity;
		}
	}

	[PunRPC]
	private void RPCPlayerContainer(int teamNumber, string nickName, string userId)
	{
		Transform parentContainer = null;
		if (teamNumber == 1)
		{
			parentContainer = bluePlayersContainer.transform;
		}
		else if (teamNumber == 2)
		{
			parentContainer = redPlayersContainer.transform;
		}
		GameObject newPlayer = Instantiate(players, parentContainer);
		newPlayer.name = userId;
		newPlayer.transform.GetChild(1).GetComponent<Text>().text = nickName;
	}

	[PunRPC]
	public void UpdateTeams(int teamNumber, int changer)
	{
		if (teamNumber == 1)
		{
			BlueTeamPlayerCount += changer;
			BlueTeamPlayerCount = Mathf.Clamp(BlueTeamPlayerCount, 0, maxPlayerCount);
		}
		else
		{
			RedTeamPlayerCount += changer;
			RedTeamPlayerCount = Mathf.Clamp(RedTeamPlayerCount, 0, maxPlayerCount);
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
				canForceSelectAbility = true;
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
					blueTeamStats.text = "Waiting For Opponents Team...";
				}
				if (RedTeamPlayerCount != maxPlayerCount)
				{
					redTeamStats.text = "Waiting For Players...";
				}
				else if (RedTeamPlayerCount == maxPlayerCount)
				{
					redTeamStats.text = "Waiting For Opponents Team...";
				}
			}

		}
	}

	#endregion


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

			// Set the players remainer
			foreach (Transform trans in bluePlayersContainer.transform)
			{
				if (trans.name == player.UserId)
				{
					Destroy(trans.gameObject);
					return;
				}
			}

			foreach (Transform trans in redPlayersContainer.transform)
			{
				if (trans.name == player.UserId)
				{
					Destroy(trans.gameObject);
					return;
				}
			}
		}
	}


	#endregion


}
