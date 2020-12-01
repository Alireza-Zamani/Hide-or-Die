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

	[Header("Top UI")]
	[SerializeField] private Text playerInfoName = null;

	[Header("Flag")]
	[SerializeField] private GameObject nameInFlagImage = null;
	[SerializeField] private GameObject roomInFlagImage = null;
	[SerializeField] private GameObject classInFlagImage = null;

	[Header("Book Rooms")]
	[SerializeField] private GameObject RandomeRoomPanel = null;
	[SerializeField] private GameObject CreatRoomPanel = null;
	[SerializeField] private GameObject JoinRoomPanel = null;

	[Header("Increase Decrease Btns")]
	[SerializeField] private Text randomeRoomCountText = null;
	[SerializeField] private Text creatRoomCountText = null;

	[Header("Enter Pun Panel")]
	[SerializeField] private InputField nickName = null;
	[SerializeField] private GameObject warningNoName = null;
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

	[Header("Join Randome Room Panel")]
	[SerializeField] private Text joinRandomePlayersCount = null;


	[Header("Join Private Room Panel")]
	[SerializeField] private GameObject joinPrivateRoomPanel = null;
	[SerializeField] private InputField joinRoomNameInputField = null;


	#region Btns

	public void OnIncreaseRoomSize(int panelNum)
	{
		int count = 1;
		switch (panelNum)
		{
			case 1:
				count = int.Parse(randomeRoomCountText.text);
				count++;
				count = Mathf.Clamp(count, 1, 5);
				randomeRoomCountText.text = count.ToString();
				break;
			case 2:
				count = int.Parse(creatRoomCountText.text);
				count++;
				count = Mathf.Clamp(count, 1, 5);
				creatRoomCountText.text = count.ToString();
				break;
		}
		teamsPlayerCount = count;
	}

	public void OnDecreaseRoomSize(int panelNum)
	{
		int count = 1;
		switch (panelNum)
		{
			case 1:
				count = int.Parse(randomeRoomCountText.text);
				count--;
				count = Mathf.Clamp(count, 1, 5);
				randomeRoomCountText.text = count.ToString();
				break;
			case 2:
				count = int.Parse(creatRoomCountText.text);
				count--;
				count = Mathf.Clamp(count, 1, 5);
				creatRoomCountText.text = count.ToString();
				break;
		}
		teamsPlayerCount = count;
	}

	public void OnBookRandomeRoomBtn()
	{
		RandomeRoomPanel.SetActive(true);
		creatRoomPanel.SetActive(false);
		joinPrivateRoomPanel.SetActive(false);
	}

	public void OnBookCreatRoomBtn()
	{
		RandomeRoomPanel.SetActive(false);
		creatRoomPanel.SetActive(true);
		joinPrivateRoomPanel.SetActive(false);
	}

	public void OnBookJoinRoomBtn()
	{
		RandomeRoomPanel.SetActive(false);
		creatRoomPanel.SetActive(false);
		joinPrivateRoomPanel.SetActive(true);
	}

	public void OnExitBtn()
	{
		Application.Quit();
	}

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

		teamsPlayerCount = int.Parse(creatRoomCountText.text);
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
		if (PlayerPrefs.HasKey("NickName"))
		{
			nickName.text = PlayerPrefs.GetString("NickName");
		}
	}

	public void ConnectedToServerBtn()
	{
		if(string.IsNullOrEmpty(nickName.text))
		{
			warningNoName.SetActive(true);
			return;
		}

		warningNoName.SetActive(false);
		playerInfoName.text = nickName.text;
		PlayerPrefs.SetString("NickName", nickName.text);
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
		teamsPlayerCount = int.Parse(randomeRoomCountText.text);
		CreateRoomOptions(roomName , true , (byte)(teamsPlayerCount * 2));
	}

	private void CreateRoomOptions(string roomName, bool isVisible, byte maxPlayersCount)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = isVisible;
		roomOptions.MaxPlayers = maxPlayersCount;
		roomOptions.PublishUserId = true;

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
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "Ranked" , rank } };

		EnterRoomParams enterRoomParams = new EnterRoomParams();
		enterRoomParams.RoomName = "Ranked Room" + Random.Range(0, 10000);
		enterRoomParams.RoomOptions = roomOptions;

		loadBalancingClient.OpCreateRoom(enterRoomParams);
	}


	public void RoomSizeChnager(int value)
	{
		teamsPlayerCount += value;
		teamsPlayerCount = Mathf.Clamp(teamsPlayerCount, 1, 5);
		joinRandomePlayersCount.text = teamsPlayerCount.ToString();
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
		nameInFlagImage.SetActive(true);
		roomInFlagImage.SetActive(false);
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
		if (autoChooseTeamToggle.isOn)
		{
			PhotonNetwork.LoadLevel("ChooseTeamAuto");
		}
		else
		{
			PhotonNetwork.LoadLevel("ChooseTeamAuto");
		}
	}

	#endregion




}
