using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Unity.Mathematics;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PunSpawner : MonoBehaviourPunCallbacks
{

	private GameObject player = null;
	private List<int> spawnedPlayers = new List<int>();

	[SerializeField] private bool isTestMode = false;

	[Header("CountDown Conditions")]
	[Range(0,60)] [SerializeField] private float countDownTimer = 10f;
	private float countDownTimerTemp = 0f;
	public float CountDownTimer { get => countDownTimer; set => countDownTimer = value; }
	[SerializeField] private Image enterGameWaitLoadBar = null;
	[SerializeField] private Text timerCounter = null;
	[SerializeField] private Text disconnectionError = null;
	[SerializeField] private GameObject timeCounterPanel = null;


	[Header("Prefabs")]
	[SerializeField] private GameObject[] playersPrefab = null;
 	[SerializeField] private GameObject bluePlayerPref = null;
	[SerializeField] private GameObject redPlayerPref = null;
	[SerializeField] private GameObject objectivePrefab = null;
	[SerializeField] private GameObject trapObjectivePrefab = null;
	[Range(0, 5)] [SerializeField] private int trapsSpawnCount = 3;

	[Header("Teams Spawn Points")]
	[SerializeField] private Transform blueTeamSpawnPoint = null;
	[SerializeField] private Transform redTeamSpawnPoint = null;

	[Header("Objectives Spawn Points")]
	[SerializeField] private Transform objectivesSpawnPoint = null;
	[SerializeField] private Transform trapDetectorObjectivesSpawnPoint = null;
	[SerializeField] private Transform[] trapObjectivesSpawnPoints = null;

	private int team = 0;
	private string className = null;

	private bool timerStarted = false;
	private float counter = 0f;
	private GameManager gameManager = null;
	private PlayerEnterGameDuty playerEnterGameDuty = null;
	private CollisionHandler collisionHandler = null;
	private IPlayer playerInterface = null;
	private int playerGroup = 0;

	private void Start()
	{
		if (isTestMode)
		{
			return;
		}
		SpawnHeros();
		gameManager = GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<GameManager>();
		countDownTimerTemp = countDownTimer;
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
		{
			Debug.LogError("No group setted for teh player");
		}
		else
		{
			playerGroup = (int)PhotonNetwork.LocalPlayer.CustomProperties["Group"];
		}

		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("CheckIfAllThePlayersHaveJoined", 10f);
		}
	}

	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (timerStarted)
			{
				counter += Time.deltaTime;
				if (counter >= 1)
				{
					counter = 0;
					CountDownTimer --;
					photonView.RPC("PlayerEnteryWaiting", RpcTarget.AllBuffered, countDownTimer);
				}
			}
		}
	}

	private void CheckIfAllThePlayersHaveJoined()
	{
		int eachTeamPlayerSize = (int)PhotonNetwork.CurrentRoom.MaxPlayers / 2;
		print(spawnedPlayers.Count);
		if((spawnedPlayers.Count - 1) != (eachTeamPlayerSize * 2))
		{
			int team1Players = 0;
			int team2Players = 0;
			foreach (int spawnedPlayersteamNumber in spawnedPlayers)
			{
				if(spawnedPlayersteamNumber == 1)
				{
					team1Players++;
				}
				else
				{
					team2Players++;
				}
			}

			int team1Remained = eachTeamPlayerSize - team1Players;
			for (int i = 1; i <= team1Remained; i++)
			{
				photonView.RPC("DisconnectedPlayerNotSpawnedSenedFromMaster", RpcTarget.AllBuffered, 1);
			}

			int team2Remained = eachTeamPlayerSize - team2Players;
			for (int i = 1; i <= team2Remained; i++)
			{
				photonView.RPC("DisconnectedPlayerNotSpawnedSenedFromMaster", RpcTarget.AllBuffered, 2);
			}
		}
	}

	private void SpawnHeros()
	{
		if (PhotonNetwork.IsConnected)
		{
			// Get the team and spawn related to that
			if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
			{
				Debug.LogError("No hashTable exists for team");
			}

			team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

			if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Group"))
			{
				Debug.LogError("No hashTable exists for team");
			}
			photonView.RPC("RPCSendTheMasterWhenSpawnedThePlayer", RpcTarget.MasterClient, (int)PhotonNetwork.LocalPlayer.CustomProperties["Group"]);


			if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"))
			{
				Debug.LogError("No hashTable exists for class");
			}

			className = (string)PhotonNetwork.LocalPlayer.CustomProperties["Class"];

			GameObject spawnablePlayerPrefab = playersPrefab[0];
			foreach (GameObject go in playersPrefab)
			{
				if(go.name == className)
				{
					spawnablePlayerPrefab = go;
					break;
				}
			}
			

			// Team Blue
			if (team == 1)
			{
				player = PhotonNetwork.Instantiate(spawnablePlayerPrefab.name, blueTeamSpawnPoint.position, Quaternion.identity);

				// Set Player Statues
				playerInterface = player.GetComponent<IPlayer>();
				playerInterface.TeamSetter("BlueTeam");
			}
			// Team Red
			else if (team == 2)
			{
				player = PhotonNetwork.Instantiate(spawnablePlayerPrefab.name, redTeamSpawnPoint.position, Quaternion.identity);

				// Set Player Statues
				playerInterface = player.GetComponent<IPlayer>();
				playerInterface.TeamSetter("RedTeam");
			}

			// Change the layer to Player so that we wont be our own enemy because at the first all of the layers are at Enemy
			player.gameObject.layer = LayerMask.NameToLayer("Player");

			if (PhotonNetwork.IsMasterClient)
			{
				SpawnObjectives();
			}

			// Set the first condition for the player
			collisionHandler = player.GetComponent<CollisionHandler>();
			collisionHandler.Team = team;
			playerEnterGameDuty = player.GetComponent<PlayerEnterGameDuty>();
			playerEnterGameDuty.Team = team;
			playerEnterGameDuty.PlayerEntered();
			StartTimerCountDown();
		}
	}

	private void StartTimerCountDown()
	{
		timeCounterPanel.SetActive(true);
		timerStarted = true;
	}

	private void SpawnObjectives()
	{
		PhotonNetwork.Instantiate(objectivePrefab.name, objectivesSpawnPoint.position, Quaternion.identity);

		GameObject newTrapDetector = PhotonNetwork.Instantiate(trapObjectivePrefab.name, trapDetectorObjectivesSpawnPoint.position, Quaternion.identity);
		newTrapDetector.GetComponent<TrapObjectiveInteractability>().TrapClassName = "TrapDetectorBeeper";

		for (int i = 0; i < trapsSpawnCount ; i++)
		{
			GameObject newTrap =  PhotonNetwork.Instantiate(trapObjectivePrefab.name, trapObjectivesSpawnPoints[i].position, Quaternion.identity);
			string trapClassName = null;
			int rand = UnityEngine.Random.Range(1 , 5);
			switch (rand)
			{
				case 1:
					trapClassName = "Miner";
					break;
				case 2:
					trapClassName = "Poisoner";
					break;
				case 3:
					trapClassName = "BearTraper";
					break;
				case 4:
					trapClassName = "Locker";
					break;
				default:
					trapClassName = "Miner";
					break;

			}
			newTrap.GetComponent<TrapObjectiveInteractability>().TrapClassName = trapClassName;
		}
	}

	[PunRPC]
	private void PlayerEnteryWaiting(float CountDownTimer)
	{
		//timerCounter.text = CountDownTimer.ToString();
		enterGameWaitLoadBar.fillAmount = (1 - CountDownTimer / countDownTimerTemp);
		if (CountDownTimer == 0)
		{
			playerEnterGameDuty.PlayerEnteredFinished();
			timeCounterPanel.SetActive(false);
			timerStarted = false;
		}
	}

	[PunRPC]
	private void DisconnectedPlayerNotSpawnedSenedFromMaster(int lostTeamNumber)
	{
		if(lostTeamNumber == 1)
		{
			gameManager.TeamGroup1RemainedPlayers--;

		}
		else if(lostTeamNumber == 2)
		{
			gameManager.TeamGroup2RemainedPlayers--;
		}
	}

	[PunRPC]
	private void RPCSendTheMasterWhenSpawnedThePlayer(int spawnedTeam)
	{
		spawnedPlayers.Add(spawnedTeam);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		if (disconnectionError.gameObject)
		{
			disconnectionError.gameObject.SetActive(true);
			disconnectionError.text = cause.ToString();
			Invoke("Disconnected", 2f);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if(otherPlayer != PhotonNetwork.LocalPlayer)
		{
			int disconnectedPlayerGroup = (int)otherPlayer.CustomProperties["Group"];
			if (disconnectedPlayerGroup == 1)
			{
				gameManager.TeamGroup1RemainedPlayers--;
			}
			else if (disconnectedPlayerGroup == 2)
			{
				gameManager.TeamGroup2RemainedPlayers--;
			}
		}
	}


	private void Disconnected()
	{
		SceneManager.LoadScene("Pun");
	}
}
