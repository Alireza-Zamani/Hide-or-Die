using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadInteractable : MonoBehaviourPunCallbacks, IInteractable
{
	public void Interact(Transform parent)
	{
		GameObject deadPlayer = gameObject.transform.GetChild(0).gameObject;
		deadPlayer.SetActive(true);
		deadPlayer.GetComponent<IPlayer>().Heal(100f);
		deadPlayer.transform.parent = null;
		Destroy(gameObject);
	}
	
}
