using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DoorInteractable : MonoBehaviourPunCallbacks, IInteractable
{

	private GameObject doorLock = null;

	public bool isLocked = false;

	private bool isOpen = false;

	private GameObject door = null;
	private GameObject doorFrame = null;

	private void Start()
	{
		door = transform.GetChild(0).gameObject;
		doorFrame = transform.GetChild(1).gameObject;
		doorLock = door.transform.GetChild(0).gameObject;
	}

	void IInteractable.Interact(Transform viewID)
	{
		if (isLocked)
		{
			return;
		}

		if (isOpen)
		{
			OpenAndCloseDoor(true);
		}
		else
		{
			OpenAndCloseDoor(false);
		}
		isOpen = !isOpen;
	}



	public bool LockDoor(bool activity)
	{
		if (isLocked == activity)
		{
			return false;
		}
		else
		{
			isLocked = activity;
			doorLock.SetActive(activity);
			isOpen = false;
			OpenAndCloseDoor(true);
			return true;
		}
	}


	private void OpenAndCloseDoor(bool activity)
	{
		door.SetActive(activity);
		doorFrame.SetActive(!activity);
	}
	
}
