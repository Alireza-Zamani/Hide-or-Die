using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DoorInteractable : MonoBehaviourPunCallbacks, IInteractable
{

	private SpriteRenderer doorSprite = null;

	public bool isLocked = false;

	private bool isOpen = false;

	private GameObject door = null;

	private void Start()
	{
		door = transform.GetChild(0).gameObject;
		doorSprite = door.GetComponent<SpriteRenderer>();
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
			
			if(activity == true)
			{
				doorSprite.color = Color.gray;
			}
			else
			{
				doorSprite.color = Color.blue;
			}
			isOpen = false;
			OpenAndCloseDoor(true);
			return true;
		}
	}


	private void OpenAndCloseDoor(bool activity)
	{
		door.SetActive(activity);
	}
	
}
