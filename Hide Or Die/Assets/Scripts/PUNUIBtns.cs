using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;

public class PUNUIBtns : MonoBehaviourPunCallbacks
{

	[Header("Classes Infos")]
	[SerializeField] private Sprite healerIcon = null;
	[SerializeField] private Sprite rammalIcon = null;
	[SerializeField] private Sprite sarbazIcon = null;
	[SerializeField] private Sprite jalladIcon = null;
	[SerializeField] private Sprite healerCharacter = null;
	[SerializeField] private Sprite rammalCharacter = null;
	[SerializeField] private Sprite sarbazCharacter = null;
	[SerializeField] private Sprite jalladCharacter = null;

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

	private float waitUntilNextTap = 0.5f;
	private float waitingtimer = 0f;
	private bool shouldWait = false;



	private void Awake()
	{
		// Get the classes that we can choose then add the to the whole abilities list
		foreach(Transform trans in blueTeamClasses.transform)
		{
			blueTeamAbilities.Add(trans.gameObject);
		}

		foreach (Transform trans in redTeamClasses.transform)
		{
			redTeamAbilities.Add(trans.gameObject);
		}
	}

	private void Update()
	{
		// If we have choosed an ability we have to wait to choose the another this prevents auto ability selecting and self ability selecting to be in the same time
		if (shouldWait)
		{
			waitingtimer += Time.deltaTime;
			if(waitingtimer >= waitUntilNextTap)
			{
				waitingtimer = 0f;
				shouldWait = false;
			}
		}
	}

	public void ClassBtn(string className)
	{
		// Wait if we have choosed any ability just right now
		if (shouldWait)
		{
			return;
		}
		shouldWait = true;
		ChoosedAbility = true;

		// See which team he is in by checking which panel is open
		if (blueTeamClasses.activeInHierarchy)
		{
			teamNumber = 1;
		}
		else
		{
			teamNumber = 2;
		}

		// Chaneg the color of ability to green only for us
		SetTheChoosedAbilityToBeGreen(className , teamNumber , true);

		// Change this ability for all players to be off
		photonView.RPC("RPCSetTheChoosedAbilities", RpcTarget.AllBuffered, className , teamNumber);

		// Set the Hashtable for the class that has been choosen this will be used while spawning the hero in the game play
		if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class") && PhotonNetwork.LocalPlayer.CustomProperties["Class"] != null)
		{
			// Chaneg the color of ability to white only for us
			SetTheChoosedAbilityToBeGreen(PhotonNetwork.LocalPlayer.CustomProperties["Class"].ToString(), teamNumber, false);

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

	private void SetTheChoosedAbilityToBeGreen(string className, int teamNumber , bool activity)
	{
		// Check which team we are in then in that team find the choosed heros cart and change its image to green cart or white cart according to activity value
		Transform parentOfClass = null;
		if (teamNumber == 1)
		{
			parentOfClass = blueTeamClasses.transform;
			foreach (Transform child in parentOfClass)
			{
				if (child.name == className)
				{
					child.gameObject.transform.GetChild(4).gameObject.SetActive(activity);
				}
			}
		}
		else if (teamNumber == 2)
		{
			parentOfClass = redTeamClasses.transform;
			foreach (Transform child in parentOfClass)
			{
				if (child.name == className)
				{
					child.gameObject.transform.GetChild(4).gameObject.SetActive(activity);
				}
			}
		}
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
				if (go.name != "Text" && go.name != "Soon" && !blueChoosedAbilities.Contains(go.name))
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

		if (!ChoosedAbility)
		{
			ClassBtn(className);
		}
	}


	#region RPCs

	[PunRPC]
	private void RPCPlayerContainer(string userId, string className)
	{
		// Set the players container statues according to the choosed ability
		foreach (Transform trans in bluePlayersContainer.transform)
		{
			if (trans.name == userId)
			{
				switch (className)
				{
					case "Healer":
						trans.GetChild(0).GetComponent<Image>().sprite = healerIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = healerCharacter;
						break;
					case "Reviver":
						trans.GetChild(0).GetComponent<Image>().sprite = rammalIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = rammalCharacter;
						break;
					case "Spearer":
						trans.GetChild(0).GetComponent<Image>().sprite = sarbazIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = sarbazCharacter;
						break;
					case "Grenader":
						trans.GetChild(0).GetComponent<Image>().sprite = jalladIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = jalladCharacter;
						break;
				}

				return;
			}
		}

		foreach (Transform trans in redPlayersContainer.transform)
		{
			if (trans.name == userId)
			{
				switch (className)
				{
					case "Healer":
						trans.GetChild(0).GetComponent<Image>().sprite = healerIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = healerCharacter;
						break;
					case "Reviver":
						trans.GetChild(0).GetComponent<Image>().sprite = rammalIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = rammalCharacter;
						break;
					case "Spearer":
						trans.GetChild(0).GetComponent<Image>().sprite = sarbazIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = sarbazCharacter;
						break;
					case "Grenader":
						trans.GetChild(0).GetComponent<Image>().sprite = jalladIcon;
						trans.GetChild(1).GetComponent<Image>().sprite = jalladCharacter;
						break;
				}

				return;
			}
		}
	}

	[PunRPC]
	private void RPCSetTheChoosedAbilities(string className, int teamNumber)
	{
		// Add the choosed ability to choosed ability list and then turn it off to be unavailable to be choosed by other players
		Transform parentOfClass = null;
		if (teamNumber == 1)
		{
			parentOfClass = blueTeamClasses.transform;
			if (!blueChoosedAbilities.Contains(className))
			{
				print(className);

				blueChoosedAbilities.Add(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.transform.GetChild(2).gameObject.SetActive(false);
						child.gameObject.transform.GetChild(3).gameObject.SetActive(true);
					}
				}
			}
		}
		else if (teamNumber == 2)
		{
			parentOfClass = redTeamClasses.transform;
			if (!redChoosedAbilities.Contains(className))
			{
				redChoosedAbilities.Add(className);
				foreach (Transform child in parentOfClass)
				{
					if (child.name == className)
					{
						child.gameObject.transform.GetChild(2).gameObject.SetActive(false);
						child.gameObject.transform.GetChild(3).gameObject.SetActive(true);
					}
				}
			}
		}
	}

	[PunRPC]
	private void RPCReSetTheChoosedAbilities(string className, int teamNumber)
	{
		// We have choosed an other ability so we have to reset the previous choosed one so we do the oposite of what we did in setting the choosed ability
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
						child.gameObject.transform.GetChild(2).gameObject.SetActive(true);
						child.gameObject.transform.GetChild(3).gameObject.SetActive(false);
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
						child.gameObject.transform.GetChild(2).gameObject.SetActive(true);
						child.gameObject.transform.GetChild(3).gameObject.SetActive(false);
					}
				}
			}
		}
	}

	#endregion



	#region Call Backs

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (otherPlayer.CustomProperties.ContainsKey("Class") && otherPlayer.CustomProperties.ContainsKey("Team"))
		{
			RPCReSetTheChoosedAbilities(otherPlayer.CustomProperties["Class"].ToString(), (int)otherPlayer.CustomProperties["Team"]);
		}
	}

	public override void OnLeftRoom()
	{
		PhotonNetwork.LocalPlayer.CustomProperties["Class"] = null;
	}


	#endregion


}
