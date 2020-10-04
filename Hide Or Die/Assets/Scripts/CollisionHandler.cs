using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollisionHandler : MonoBehaviourPunCallbacks
{

	private GameObject actionBtn = null;
	private BuyAvailibility buyAvailibility = null;

	private void Start()
	{
		if (!photonView.IsMine)
		{
			Destroy(this);
		}
		actionBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).gameObject;
		buyAvailibility = GetComponent<BuyAvailibility>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// If entered an interactable one then update action btn
		if(other.tag == "Interactable")
		{
			UpdateActionBtn(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		// If we have exited an interactable object turn off the action btn but if the object is our child then dont because we want to redo the action later
		if (other.tag == "Interactable" && other.gameObject.transform.parent != transform && transform.childCount == 0)
		{
			UpdateActionBtn(false);
		}

		//If we exited the buy zone we have to disable buy button
		if(other.tag == "BuyZone")
		{
			buyAvailibility.CanBuy = false;
			Destroy(other.gameObject);
		}
	}

	private void UpdateActionBtn(bool activity)
	{
		actionBtn.SetActive(activity);
	}

}
