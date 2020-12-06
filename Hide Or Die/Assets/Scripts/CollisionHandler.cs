using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviourPunCallbacks
{
	private int hasLock = 0;
	public int HasLock { get => hasLock; set => hasLock = value; }

	private bool hasExitedBuyZone = false;
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
		if(other.tag == "ObjectiveReachPoint" && gameObject.tag == "BlueTeam" && transform.childCount != 1)
		{
			foreach(Transform trans in transform)
			{
				if(trans.gameObject.tag == "Interactable")
				{
					PlayerMatchData playerInterface = GetComponent<PlayerMatchData>();
					playerInterface.ObjectiveReached();
					break;
				}
			}
			
		}

		// If entered an interactable one then update action btn
		if (other.tag == "Interactable")
		{
			UpdateActionBtn(true);

			if (HasLock != 0)
			{
				// If the other is the door
				DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
				if (doorInteractable != null && Team == 2)
				{
					UpdateLockBtn(true, doorInteractable);
				}
			}
			
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (HasLock != 0)
		{
			DoorInteractable doorInteractable = other.gameObject.GetComponent<DoorInteractable>();
			if (doorInteractable != null && Team == 2)
			{
				UpdateLockBtn(false, doorInteractable);
			}
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
			if (gameObject.tag == "BlueTeam" && !hasExitedBuyZone)
			{
				other.gameObject.tag = "ObjectiveReachPoint";
				hasExitedBuyZone = true;
			}
			else
			{
				Destroy(other.gameObject);
			}
		}
	}

	private void UpdateActionBtn(bool activity)
	{
		if (photonView.IsMine)
		{
			if(actionBtn == null)
			{
				actionBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(1).gameObject;
			}
			actionBtn.SetActive(activity);
		}
	}

	public void UpdateLockBtn(bool activity , DoorInteractable doorInteractable)
	{
		lockBtn.SetActive(activity);

		if(doorInteractable != null)
		{
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

}
