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



	private void Start()
	{
		CheckTeamCapacity();
	}

	public void ChooseTeam(int teamNumber)
	{
		// Set the Hashtable for team
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

		// Call RPC functions to call in all players
		TeamNum = teamNumber;
		photonView.RPC("UpdateTeams", RpcTarget.AllBuffered, teamNumber);
		photonView.RPC("UpdateStats", RpcTarget.AllBuffered, teamNumber);
		if (teamNumber == 1)
		{
			SetGameObjectActivity(blueTeamPanel, true);
			blueTeamReaminer.text = BlueTeamPlayerCount.ToString();
		}
		else
		{
			SetGameObjectActivity(redTeamPanel, true);
			redTeamReaminer.text = RedTeamPlayerCount.ToString();
		}
	}

	private void SetGameObjectActivity(GameObject panelName , bool activity)
	{
		panelName.SetActive(activity);
	}

	private void CheckTeamCapacity()
	{
		// See if the team is full set its buttons interactibility to false
		if (BlueTeamPlayerCount == maxPlayerCount)
		{
			blueTeamButton.GetComponent<Button>().interactable = false;
		}
		if (RedTeamPlayerCount == maxPlayerCount)
		{
			redTeamButton.GetComponent<Button>().interactable = false;
		}
	}

	[PunRPC]
	public void UpdateTeams(int teamNumber)
	{
		if(teamNumber == 1)
		{
			BlueTeamPlayerCount++;
			CheckTeamCapacity();
		}
		else
		{
			RedTeamPlayerCount++;
			CheckTeamCapacity();
		}
	}

	[PunRPC]
	public void UpdateStats(int teamNumber)
	{
		if(TeamNum != 0)
		{
			if(BlueTeamPlayerCount == maxPlayerCount && RedTeamPlayerCount == maxPlayerCount)
			{
				blueTeamStats.text = "Starting The Match...";
				redTeamStats.text = "Starting The Match...";
				if (PhotonNetwork.IsMasterClient)
				{
					SceneManager.LoadScene("MainScene");
				}
			}
			else if(BlueTeamPlayerCount == maxPlayerCount)
			{
				blueTeamStats.text = "We are Full Waiting For Epponents Players...";
			}
			else if (RedTeamPlayerCount == maxPlayerCount)
			{
				redTeamStats.text = "We are Full Waiting For Epponents Players...";
			}
		}
	}


	public override void OnPlayerEnteredRoom(Player player)
	{
		print(player.NickName + "  Entered in   << " + PhotonNetwork.CurrentRoom.Name + " >>  And rooms player count is : " + PhotonNetwork.CurrentRoom.PlayerCount);
	}


}
