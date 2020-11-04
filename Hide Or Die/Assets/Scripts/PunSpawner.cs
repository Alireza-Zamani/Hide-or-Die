using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Unity.Mathematics;

public class PunSpawner : MonoBehaviourPunCallbacks
{

	private GameObject player = null;

	[SerializeField] private bool isTestMode = false;

	[Header("CountDown Conditions")]
	[Range(0,60)] [SerializeField] private float countDownTimer = 10f;
	public float CountDownTimer { get => countDownTimer; set => countDownTimer = value; }
	[SerializeField] private Text timerCounter = null;
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
	private PlayerEnterGameDuty playerEnterGameDuty = null;
	private CollisionHandler collisionHandler = null;
	private IPlayer playerInterface = null;


	private void Start()
	{
		if (isTestMode)
		{
			return;
		}
		SpawnHeros();
	}

	private void Update()
	{
		if (timerStarted)
		{
			counter += Time.deltaTime;
			if(counter >= 1)
			{
				counter = 0;
				CountDownTimer -= 1;
				timerCounter.text = CountDownTimer.ToString();
				if(CountDownTimer == 0)
				{
					playerEnterGameDuty.PlayerEnteredFinished();
					timeCounterPanel.SetActive(false);
					timerStarted = false;
				}
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


			/*
			// Team Blue
			if(team == 1)
			{
				player = PhotonNetwork.Instantiate(bluePlayerPref.name, blueTeamSpawnPoint.position , Quaternion.identity);

				// Set Player Statues
				playerInterface = player.GetComponent<IPlayer>();
				playerInterface.TeamSetter("BlueTeam");
			}
			// Team Red
			else if(team == 2)
			{
				player = PhotonNetwork.Instantiate(redPlayerPref.name, redTeamSpawnPoint.position, Quaternion.identity);

				// Set Player Statues
				playerInterface = player.GetComponent<IPlayer>();
				playerInterface.TeamSetter("RedTeam");
			}
			*/

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
}
