using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMatchData : MonoBehaviourPunCallbacks , IPlayer
{

	private bool hasTrap = false;

	public LayerMask raycastableForInSightLayerMask = new LayerMask();
	public LayerMask detectableLayerMask = new LayerMask();

	private Image healthBarMain = null;
	private Image abilityCoolDownBarMain = null;
	private GameObject canvas = null;
	private GameObject healthBar = null;
	private SpriteRenderer healthBarSprite = null;
	private AnimatorController animatorController = null;

	[SerializeField] private GameObject deathVfx = null;
	[SerializeField] private GameObject bodyHandler = null;
	[SerializeField] private GameObject spectDeathDrone = null;

	[SerializeField] private float health = 100;

	public float Health { get => health; set => health = value; }
	
	private string team = null;

	private string className = null;


	[HideInInspector]
	public GameManager gameManager = null;

	private PlayerMovement playerMovement = null;

	public int playerGroup = 0;

	private Button abilityBtn = null;
	private Button actionBtn = null;

	private float abilityCoolDownTimer = 10f;
	private float abilitytimer = 0f;
	private bool abilityUsed = false;

	private bool canTakeDamage = true;
	public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

	private void Awake()
	{
		animatorController = GetComponent<AnimatorController>();

		healthBar = transform.GetChild(0).gameObject;
		healthBarSprite = healthBar.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
		AddAbility();
	}


	private void Start()
	{
		playerMovement = GetComponent<PlayerMovement>();
		canvas = GameObject.FindGameObjectWithTag("UI").gameObject;

		actionBtn = canvas.transform.GetChild(1).transform.gameObject.GetComponent<Button>();

		abilityBtn = canvas.transform.GetChild(2).transform.gameObject.GetComponent<Button>();

		healthBarMain = canvas.transform.GetChild(11).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
		abilityCoolDownBarMain = canvas.transform.GetChild(11).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();

		gameManager = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<GameManager>();
		if (photonView.IsMine)
		{
			healthBar.SetActive(false);
			SetThePlayersGroup();
		}
	}


	private void Update()
	{
		if (abilityUsed)
		{
			abilitytimer += Time.deltaTime;
			abilityCoolDownBarMain.fillAmount = (abilitytimer / abilityCoolDownTimer);
			if(abilitytimer >= abilityCoolDownTimer)
			{
				abilitytimer = 0f;
				abilityUsed = false;
				abilityBtn.interactable = true;
			}
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
			photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
		}
	}


	public void Die()
	{

		if (photonView.IsMine)
		{
			PhotonNetwork.Instantiate(deathVfx.name, new Vector3(transform.position.x, transform.position.y, deathVfx.transform.position.z), Quaternion.identity);
			GameObject newBodyHandler = PhotonNetwork.Instantiate(bodyHandler.name, transform.position, Quaternion.identity);
			newBodyHandler.GetComponent<DeadBodyHandler>().AtatchTheDeadBody(photonView.ViewID);

			// Set the spect drone active
			GameObject newSpectDeathDrone = PhotonNetwork.Instantiate(spectDeathDrone.name, transform.position, Quaternion.identity);
			newBodyHandler.GetComponent<DeadBodyHandler>().SpectDeathDrone = newSpectDeathDrone;

			// Set the canvas to not active
			newBodyHandler.GetComponent<DeadBodyHandler>().Canavs = canvas;
			for (int i = 1; i < canvas.transform.childCount; i++)
			{
				Button btn = canvas.transform.GetChild(i).gameObject.GetComponent<Button>();
				if (btn != null)
				{
					btn.interactable = false;
				}
			}

		}

		// Update the all players of teams in the game manager
		if (playerGroup == 1)
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

	public void AbilityUsed(float abilityCoolDownValue)
	{
		abilityCoolDownTimer = abilityCoolDownValue;
		abilityUsed = true;
		abilityCoolDownBarMain.fillAmount = 0f;
		abilityBtn.interactable = false;
	}

	public void ActionBtnTurnOnOrOff(bool availibility)
	{
		actionBtn.gameObject.SetActive(availibility);
	}

	private void ResetStucked()
	{
		playerMovement.IsStucked = false;
	}

	[PunRPC]
	private void RPCSetThePlayerGroup(int playerGroup)
	{
		this.playerGroup = playerGroup;
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
		// Animation
		animatorController.CanTakeHit();
		healthBar.transform.localScale = new Vector3(Health / 100f , healthBar.transform.localScale.y, healthBar.transform.localScale.z);
		healthBarSprite.color = Color.Lerp(Color.red, Color.green, healthBar.transform.localScale.x);

		if (photonView.IsMine)
		{
			healthBarMain.fillAmount = (Health / 100f);
		}

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
		if (photonView.IsMine)
		{
			healthBarMain.fillAmount = (Health / 100f);
		}
	}

	
}
