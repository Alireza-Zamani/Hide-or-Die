using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DoorInteractable : MonoBehaviourPunCallbacks, IInteractable
{

	private bool isOpen = false;

	private GameObject door = null;

	private void Start()
	{
		door = transform.GetChild(0).gameObject;
	}

	void IInteractable.Interact(Transform viewID)
	{
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

	private void OpenAndCloseDoor(bool activity)
	{
		door.SetActive(activity);
	}
	
}
