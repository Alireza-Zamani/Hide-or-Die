using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadBodyHandler : MonoBehaviourPunCallbacks
{


	private GameObject canvas = null;
	public GameObject Canavs { get => canvas; set => canvas = value; }

	[ContextMenu("Revive")]
	public void ResetPlayer()
	{
		if (photonView.IsMine)
		{
			if (!Canavs.activeInHierarchy)
			{
				Canavs.SetActive(true);
			}
		}
		GameObject deadPlayer = gameObject.transform.GetChild(0).gameObject;
		deadPlayer.SetActive(true);
		deadPlayer.GetComponent<IPlayer>().Heal(100f);
		deadPlayer.transform.parent = null;
		Destroy(gameObject);
	}
}
