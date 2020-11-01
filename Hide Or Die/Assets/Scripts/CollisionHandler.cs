using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviourPunCallbacks
{

	private GameObject actionBtn = null;
	private GameObject lockBtn = null;
	private Text lockBtnText = null;
	private BuyAvailibility buyAvailibility = null;
	private int team = 0;
	public int Team { get => team; set => team = value; }

	private void Start()
	{
		if (!photonView.IsMine)
		{
			Destroy(this);
		}
		
		actionBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).gameObject;
		lockBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(5).gameObject;
		lockBtnText = lockBtn.transform.GetChild(0).gameObject.GetComponent<Text>();
		buyAvailibility = GetComponent<BuyAvailibility>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// Check if we have triggered the objective reachpoint and we have objective with ourself
		if(other.tag == "ObjectiveReachPoint" && gameObject.tag == "BlueTeam" && transform.childCount != 1 && transform.GetChild(1).gameObject.tag == "Interactable")
		{
			PlayerMatchData playerInterface = GetComponent<PlayerMatchData>();
			playerInterface.ObjectiveReached();
		}

		// If entered an interactable one then update action btn
		if (other.tag == "Interactable")
		{
			UpdateActionBtn(true);

			// If the other is the door
			DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
			if (doorInteractable != null && Team == 2)
			{
				UpdateLockBtn(true, doorInteractable);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
		if (doorInteractable != null && Team == 2)
		{
			UpdateLockBtn(false, doorInteractable);
		}

		// If we have exited an interactable object turn off the action btn but if the object is our child then dont because we want to redo the action later
		if (other.tag == "Interactable" && other.gameObject.transform.parent != transform && transform.childCount == 1)
		{
			UpdateActionBtn(false);
		}

		//If we exited the buy zone we have to disable buy button
		if(other.tag == "BuyZone")
		{
			buyAvailibility.CanBuy = false;
			if(gameObject.tag == "BlueTeam")
			{
				other.gameObject.tag = "ObjectiveReachPoint";
			}
			else
			{
				Destroy(other.gameObject);
			}
		}
	}

	private void UpdateActionBtn(bool activity)
	{
		actionBtn.SetActive(activity);
	}

	private void UpdateLockBtn(bool activity , DoorInteractable doorInteractable)
	{
		lockBtn.SetActive(activity);
		if (doorInteractable.isLocked)
		{
			lockBtnText.text = "UnLock";
		}
		else
		{
			lockBtnText.text = "Lock";
		}
	}

}
