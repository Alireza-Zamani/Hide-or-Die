using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;

public class PUNUIBtns : MonoBehaviourPunCallbacks
{

	[Header("Players Status")]
	[SerializeField] private GameObject bluePlayersContainer = null;
	[SerializeField] private GameObject redPlayersContainer = null;

	[Header("Classes Status")]
	[SerializeField] private GameObject blueTeamClasses = null;
	[SerializeField] private GameObject redTeamClasses = null;

	private List<GameObject> blueTeamAbilities = new List<GameObject>();
	private List<GameObject> redTeamAbilities = new List<GameObject>();

	private int teamNumber = 0;

	private List<string> blueChoosedAbilities = new List<string>();
	private List<string> redChoosedAbilities = new List<string>();

	private bool choosedAbility = false;
	public bool ChoosedAbility { get => choosedAbility; set => choosedAbility = value; }


	private void Awake()
	{
		foreach(Transform trans in blueTeamClasses.transform)
		{
			blueTeamAbilities.Add(trans.gameObject);
		}

		foreach (Transform trans in redTeamClasses.transform)
		{
			redTeamAbilities.Add(trans.gameObject);
		}
	}

	public void ClassBtn(string className)
	{
		ChoosedAbility = true;
		// See which team he is in
		if (blueTeamClasses.activeInHierarchy)
		{
			teamNumber = 1;
		}
		else
		{
			teamNumber = 2;
		}

		// change this ability for all players to be off
		photonView.RPC("RPCSetTheChoosedAbilities", RpcTarget.AllBuffered, className , teamNumber);

		// Set the Hashtable for team
		if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"))
		{
			// change this ability for all players to be off because we have selected another ability
			photonView.RPC("RPCReSetTheChoosedAbilities", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.CustomProperties["Class"], teamNumber);
			PhotonNetwork.LocalPlayer.CustomProperties["Class"] = className;
		}
		else
		{
			ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Class" , className }
			};

			PhotonNetwork.SetPlayerCustomProperties(playerProps);
		}

		// Change the player containers abilites
		photonView.RPC("RPCPlayerContainer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId , className);
	}

	public void AutoSelectAbility()
	{
		string className = null;

		// See which team he is in
		if (blueTeamClasses.activeInHierarchy)
		{
			// See which ability is free to choose
			foreach (GameObject go in blueTeamAbilities)
			{
				if (go.name != "Text" && !blueChoosedAbilities.Contains(go.name))
				{
					className = go.name;
					break;
				}
			}
		}
		else
		{
			// See which ability is free to choose
			foreach (GameObject go in redTeamAbilities)
			{
				if (go.name != "Text" && !redChoosedAbilities.Contains(go.name))
				{
					className = go.name;
					break;
				}
			}
		}

		ClassBtn(className);
	}

	[PunRPC]
	private void RPCPlayerContainer(string userId , string className)
	{
		// Set the players remainer
		foreach (Transform trans in bluePlayersContainer.transform)
		{
			if (trans.name == userId)
			{
				trans.GetChild(3).GetComponent<Text>().text = className;
				return;
			}
		}

		foreach (Transform trans in redPlayersContainer.transform)
		{
			if (trans.name == userId)
			{
				trans.GetChild(3).GetComponent<Text>().text = className;
				return;
			}
		}
	}


	[PunRPC]
	private void RPCSetTheChoosedAbilities(string className , int teamNumber)
	{
		
		Transform parentOfClass = null;
		if (teamNumber == 1)
		{
			parentOfClass = blueTeamClasses.transform;
			if (!blueChoosedAbilities.Contains(className))
			{
				blueChoosedAbilities.Add(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.GetComponent<Button>().interactable = false;
					}
				}
			}
		}
		else if(teamNumber == 2)
		{
			parentOfClass = redTeamClasses.transform;
			if (!redChoosedAbilities.Contains(className))
			{
				redChoosedAbilities.Add(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.GetComponent<Button>().interactable = false;
					}
				}
			}
		}
	}

	[PunRPC]
	private void RPCReSetTheChoosedAbilities(string className, int teamNumber)
	{

		Transform parentOfClass = null;
		if (teamNumber == 1)
		{
			parentOfClass = blueTeamClasses.transform;
			if (blueChoosedAbilities.Contains(className))
			{
				blueChoosedAbilities.Remove(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.GetComponent<Button>().interactable = true;
					}
				}
			}
		}
		else if (teamNumber == 2)
		{
			parentOfClass = redTeamClasses.transform;
			if (redChoosedAbilities.Contains(className))
			{
				redChoosedAbilities.Remove(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.GetComponent<Button>().interactable = true;
					}
				}
			}
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer.CustomProperties.ContainsKey("Class") && otherPlayer.CustomProperties.ContainsKey("Team"))
		{
			RPCReSetTheChoosedAbilities(otherPlayer.CustomProperties["Class"].ToString(), (int)otherPlayer.CustomProperties["Team"]);
		}
	}

}
