using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PunManager : MonoBehaviourPunCallbacks
{

	private LoadBalancingClient loadBalancingClient = new LoadBalancingClient();

	[Header("MatchMaking Type")]
	[SerializeField] private Toggle autoChooseTeamToggle = null;

	[Header("Enter Pun Panel")]
	[SerializeField] private InputField nickName = null;
	[SerializeField] private GameObject connectPanel = null;
	[SerializeField] private GameObject waitingPanel = null;
	[SerializeField] private GameObject joinPanel = null;

	[Header("Create Room Panel")]
	[SerializeField] private GameObject creatRoomPanel = null;
	[SerializeField] private Toggle privateRoomToggle = null;
	[SerializeField] private InputField roomNameInputField = null;
	[SerializeField] private Text randomeCode = null;
	private int teamsPlayerCount = 1;
	private bool playersCounthasChoosed = false;

	[Header("Join Private Room Panel")]
	[SerializeField] private GameObject joinPrivateRoomPanel = null;
	[SerializeField] private InputField joinRoomNameInputField = null;


	#region Btns

	public virtual void OnJoinRandomeRoomBtn()
	{
		// If we had a room in lobby then join it else create one now
		joinPanel.SetActive(false);
		waitingPanel.SetActive(true);
		JoinRandomRoom();
	}

	public virtual void OnCreateRoomBtn()
	{
		randomeCode.text = Random.Range(0, 10000).ToString();
		creatRoomPanel.SetActive(true);
	}

	public virtual void OnJoinPrivateRoomBtn()
	{
		joinPrivateRoomPanel.SetActive(true);
	}

	public void OnToggleValueChanged()
	{
		roomNameInputField.gameObject.SetActive(privateRoomToggle.isOn);
	}

	public void OnCreateBtn()
	{
		if (!playersCounthasChoosed)
		{
			print("Room player count is empty");
			return;
		}
		string roomName = null;
		if (privateRoomToggle.isOn)
		{
			if (string.IsNullOrEmpty(roomNameInputField.text))
			{
				print("Room name is empty");
				return;
			}
			roomName = roomNameInputField.text + randomeCode.text;
		}
		else
		{
			roomName = "Public Room" + randomeCode.text;
		}

		CreateRoomOptions(roomName , !privateRoomToggle.isOn , (byte)(teamsPlayerCount * 2));
	}

	public void OnPlayerCountBtn(int playerCount)
	{
		playersCounthasChoosed = true;
		teamsPlayerCount = playerCount;
	}

	public void OnJoinBtn()
	{
		if (string.IsNullOrEmpty(joinRoomNameInputField.text))
		{
			print("Room name is empty");
			return;
		}
		JoinRoom();
	}

	#endregion




	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public void ConnectedToServerBtn()
	{
		if(string.IsNullOrEmpty(nickName.text))
		{
			print("Nick name is empty");
			return;
		}

		// Set the local players stats and then connect to photon
		PhotonNetwork.LocalPlayer.NickName = nickName.text;
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
		}
		PhotonNetwork.ConnectUsingSettings();
		connectPanel.SetActive(false);
		waitingPanel.SetActive(true);
	}

	private void CreateRoom()
	{
		// Create a room with given room options
		string roomName = "Room" + Random.Range(0, 10000);
		CreateRoomOptions(roomName , true , (byte)(teamsPlayerCount * 2));
	}

	private void CreateRoomOptions(string roomName, bool isVisible, byte maxPlayersCount)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = isVisible;
		roomOptions.MaxPlayers = maxPlayersCount;

		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	private void JoinRandomRoom()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	private void JoinRoom()
	{
		PhotonNetwork.JoinRoom(joinRoomNameInputField.text);
	}

	private void JoinRankedRoom(byte maxPlayerCount , byte rank)
	{
		OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
		opJoinRandomRoomParams.ExpectedMaxPlayers = maxPlayerCount;
		opJoinRandomRoomParams.ExpectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable{{"Rank" , rank } };

		loadBalancingClient.OpJoinRandomRoom(opJoinRandomRoomParams);
	}

	private void CreateRankedRoom(byte maxPlayerCount, byte rank , bool isOpen , bool isVisible)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = isOpen;
		roomOptions.IsVisible = isVisible;
		roomOptions.MaxPlayers = maxPlayerCount;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "Ranked" , rank } };

		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = "Ranked Room" + Random.Range(0, 10000);
		enterRoomParams.RoomOptions = roomOptions;

		loadBalancingClient.OpCreateRoom(enterRoomParams);
	}


	
	#region CallBacks

	public override void OnConnected()
	{
		//print("Connected to internet");
	}

	public override void OnConnectedToMaster()
	{
		//print(PhotonNetwork.NickName + " ====>  Connected to server");
		waitingPanel.SetActive(false);
		joinPanel.SetActive(true);
	}


	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		//print("There was no room trying to create one");
		CreateRoom();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		print("Entered room name couldnt found please try again !!!");
	}

	public override void OnJoinedRoom()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			print(player.NickName + "  Is in   << " + PhotonNetwork.CurrentRoom.Name + " >>  And player count is" + PhotonNetwork.CurrentRoom.PlayerCount);
		}
		if (autoChooseTeamToggle.isOn)
		{
			PhotonNetwork.LoadLevel("ChooseTeamAuto");
		}
		else
		{
			PhotonNetwork.LoadLevel("ChooseTeam");
		}
	}

	#endregion




}
