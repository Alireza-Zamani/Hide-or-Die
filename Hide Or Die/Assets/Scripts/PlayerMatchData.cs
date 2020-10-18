using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMatchData : MonoBehaviour , IPlayer
{
	private GameObject canvas = null;

	[SerializeField] private GameObject bodyHandler = null;

	[SerializeField] private float health = 100;

	public float Health { get => health; set => health = value; }

	private string team = null;

	private string className = null;

	private PhotonView photonView;

	private GameManager gameManager = null;

	private int playerGroup = 0;

	private void Awake()
	{
		AddAbility();
	}


	private void Start()
	{
		photonView = PhotonView.Get(this);
		canvas = GameObject.FindGameObjectWithTag("UI").gameObject;
		gameManager = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<GameManager>();
		if (photonView.IsMine)
		{
			SetThePlayersGroup();
		}
	}

	private void SetThePlayersGroup()
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			Debug.LogError("No group setted for teh player");
		}
		else
		{
			playerGroup = (int)PhotonNetwork.LocalPlayer.CustomProperties["Group"];
			photonView.RPC("RPCSetThePlayerGroup", RpcTarget.OthersBuffered, playerGroup);
		}

	}

	[PunRPC]
	private void RPCSetThePlayerGroup(int playerGroup)
	{
		this.playerGroup = playerGroup;
	}

	public void AddAbility()
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"))
		{
			ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
			{
				{"Class" , "Grenader" }
			};

			PhotonNetwork.SetPlayerCustomProperties(playerProps);
		}


		className = (string)PhotonNetwork.LocalPlayer.CustomProperties["Class"];

		switch (className)
		{
			case "Healer":
				gameObject.AddComponent<AbilityHealer>();
				break;
			case "Spearer":
				gameObject.AddComponent<AbilitySpear>();
				break;
			case "Grenader":
				gameObject.AddComponent<AbilityGrenade>();
				break;
			case "Teleporter":
				gameObject.AddComponent<AbilityTeleport>();
				break;
			case "Reviver":
				gameObject.AddComponent<AbilityRevive>();
				break;
			case "Thief":
				gameObject.AddComponent<AbilityThief>();
				break;
			case "Guard":
				gameObject.AddComponent<AbilityDoubleDamage>();
				break;

			// If no ability was choosed we will give the player grenade ability automaticlly
			default:
				gameObject.AddComponent<AbilityGrenade>();
				break;
		}
	}

	public string TeamGetter()
	{
		return team;
	}

	public void TeamSetter(string team)
	{
		this.team = team;
	}

	public void TakeDamage(float damageAmount)
	{
		photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
	}


	public void Die()
	{
		GameObject newBodyHandler = Instantiate(bodyHandler, transform.position, Quaternion.identity);
		if (photonView.IsMine)
		{
			if (canvas.activeInHierarchy)
			{
				newBodyHandler.GetComponent<DeadBodyHandler>().Canavs = canvas;
				canvas.SetActive(false);
			}
		}
		transform.parent = newBodyHandler.transform;


		// Update the all players of teams in the game manager
		if(playerGroup == 1)
		{
			gameManager.TeamGroup1RemainedPlayers--;
		}
		else if(playerGroup == 2)
		{
			gameManager.TeamGroup2RemainedPlayers--;
		}

		gameObject.SetActive(false);
	}

	public void ObjectiveReached()
	{
		photonView.RPC("RPCObjectiveReached", RpcTarget.AllBuffered);
	}



	public void Heal(float healAmount)
	{
		photonView.RPC("RPCHeal", RpcTarget.AllBuffered, healAmount);
	}


	[PunRPC]
	private void RPCObjectiveReached()
	{
		// Update the all players of teams in the game manager
		// Update the all players of teams in the game manager
		if (playerGroup == 1)
		{
			gameManager.TeamGroup2RemainedPlayers = 0;
		}
		else if (playerGroup == 2)
		{
			gameManager.TeamGroup1RemainedPlayers = 0;
		}

		gameObject.SetActive(false);
	}


	[PunRPC]
	public void RPCTakeDamage(float damageAmount)
	{
		Health -= damageAmount;

		if (Health <= 0)
		{
			Die();
		}
	}

	[PunRPC]
	public void RPCHeal(float damageAmount)
	{
		Health += damageAmount;
		Health = Mathf.Clamp(Health , 0 , 100);
	}
}
