using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextMatch : MonoBehaviour
{

	private void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Invoke("CallNextLevel", 2f);
		}
	}

	private void CallNextLevel()
	{
		PhotonNetwork.LoadLevel("MainScene");
	}

}
