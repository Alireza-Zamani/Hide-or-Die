using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PunManager : MonoBehaviourPunCallbacks
{

	[SerializeField] private InputField nickName = null;
	[SerializeField] private GameObject connectPanel = null;
	[SerializeField] private GameObject waitingPanel = null;
	[SerializeField] private GameObject joinPanel = null;


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
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
			connectPanel.SetActive(false);
			waitingPanel.SetActive(true);
		}
	}

	public virtual void CreateOrJoinRandomRoom()
	{
		// If we had a room in lobby then join it else create one now
		joinPanel.SetActive(false);
		waitingPanel.SetActive(true);
		PhotonNetwork.JoinRandomRoom();
	}

	private void CreateRoom()
	{
		// Create a room with given room options
		string roomName = "Room" + Random.Range(0, 1000);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.MaxPlayers = 2;
		PhotonNetwork.CreateRoom(roomName, roomOptions);
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

	public override void OnJoinedRoom()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			print(player.NickName + "  Is in   << " + PhotonNetwork.CurrentRoom.Name + " >>  And player count is : " + PhotonNetwork.CurrentRoom.PlayerCount);
		}
		PhotonNetwork.LoadLevel("ChooseTeam");
	}




	#endregion




}
