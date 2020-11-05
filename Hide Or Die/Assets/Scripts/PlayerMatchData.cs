using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMatchData : MonoBehaviour , IPlayer
{

	private bool hasTrap = false;

	public LayerMask raycastableForInSightLayerMask = new LayerMask();
	public LayerMask detectableLayerMask = new LayerMask();

	private GameObject canvas = null;
	private GameObject healthBar = null;
	private SpriteRenderer healthBarSprite = null;

	[SerializeField] private GameObject bodyHandler = null;

	[SerializeField] private float health = 100;

	public float Health { get => health; set => health = value; }
	
	private string team = null;

	private string className = null;

	private PhotonView photonView;

	private GameManager gameManager = null;

	private PlayerMovement playerMovement = null;

	private int playerGroup = 0;

	private bool canTakeDamage = true;
	public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

	private void Awake()
	{
		healthBar = transform.GetChild(0).gameObject;
		healthBarSprite = healthBar.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
		AddAbility();
	}


	private void Start()
	{
		photonView = PhotonView.Get(this);
		playerMovement = GetComponent<PlayerMovement>();
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
		if (CanTakeDamage)
		{
			print("Took Damage >>>  " + damageAmount);
			photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
		}

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


	public void AddComponent(string component)
	{
		// UI
		if (photonView.IsMine)
		{
			if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Team"] == 2)
			{
				canvas.transform.GetChild(6).gameObject.SetActive(true);
			}
		}
		
		switch (component)
		{
			case "Miner":
				gameObject.AddComponent<Miner>();
				break;
			case "Poisoner":
				gameObject.AddComponent<Poisoner>();
				break;
			case "BearTraper":
				gameObject.AddComponent<BearTraper>();
				break;
			case "TrapDetectorBeeper":
				gameObject.AddComponent<TrapDetectorBeeper>();
				break;
			case "Locker":
				if (photonView.IsMine)
				{
					GetComponent<CollisionHandler>().HasLock = 2;
					canvas.transform.GetChild(6).gameObject.SetActive(false);
				}
				break;
		}
	}

	public bool HasTrapGetter()
	{
		return hasTrap;
	}

	public void HasTrapSetter(bool activity)
	{
		hasTrap = activity;
	}

	public void StuckPlayer(float timeRate)
	{
		playerMovement.IsStucked = true;
		Invoke("ResetStucked", timeRate);
	}


	private void ResetStucked()
	{
		playerMovement.IsStucked = false;
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
		if (!canTakeDamage)
			return;
		
		Health -= damageAmount;

		healthBar.transform.localScale = new Vector3(Health / 100f , healthBar.transform.localScale.y, healthBar.transform.localScale.z);
		healthBarSprite.color = Color.Lerp(Color.red, Color.green, healthBar.transform.localScale.x);

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
		healthBar.transform.localScale = new Vector3(Health / 100f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
		healthBarSprite.color = Color.Lerp(Color.red, Color.green, healthBar.transform.localScale.x);
	}

	
}
