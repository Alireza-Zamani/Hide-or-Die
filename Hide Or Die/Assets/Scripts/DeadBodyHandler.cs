using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DeadBodyHandler : MonoBehaviourPunCallbacks
{


	private GameObject canvas = null;
	public GameObject Canavs { get => canvas; set => canvas = value; }

	private GameObject spectDeathDrone = null;
	public GameObject SpectDeathDrone { get => spectDeathDrone; set => spectDeathDrone = value; }


	public void AtatchTheDeadBody(int photonID)
	{
		photonView.RPC("RPCAttachTheDeadBody", RpcTarget.AllBuffered , photonID);
	}

	public void ResetPlayer()
	{
		photonView.RPC("RPCResetPlayer", RpcTarget.AllBuffered);
	}


	[PunRPC]
	private void RPCAttachTheDeadBody(int photonID)
	{
		PhotonView.Find(photonID).transform.parent = gameObject.transform;
	}

	[PunRPC]
	private void RPCResetPlayer()
	{
		GameObject deadPlayer = gameObject.transform.GetChild(0).gameObject;
		deadPlayer.SetActive(true);
		deadPlayer.GetComponent<IPlayer>().Heal(100f);
		deadPlayer.transform.parent = null;

		if (SpectDeathDrone != null)
		{
			deadPlayer.GetComponent<MovementAbstract>().SetTheFollowTarget();
			Destroy(SpectDeathDrone);
			for (int i = 1; i < canvas.transform.childCount; i++)
			{
				Button btn = canvas.transform.GetChild(i).gameObject.GetComponent<Button>();
				if (btn != null)
				{
					btn.interactable = true;
				}
			}
		}

		PlayerMatchData playerMatchData = deadPlayer.GetComponent<PlayerMatchData>();

		// Update the all players of teams in the game manager
		if (playerMatchData.playerGroup == 1)
		{
			playerMatchData.gameManager.TeamGroup1RemainedPlayers++;
		}
		else if (playerMatchData.playerGroup == 2)
		{
			playerMatchData.gameManager.TeamGroup2RemainedPlayers++;
		}

		Destroy(gameObject);
	}
}
