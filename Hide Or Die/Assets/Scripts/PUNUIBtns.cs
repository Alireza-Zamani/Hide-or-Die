using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PUNUIBtns : MonoBehaviour
{
	
	public void ClassBtn(string className)
	{
		// Set the Hashtable for team
		if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"))
		{
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
	}

}
