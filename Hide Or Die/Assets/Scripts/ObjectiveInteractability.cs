using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ObjectiveInteractability : MonoBehaviourPunCallbacks, IInteractable
{

	private bool isGrabbed = false;

	void IInteractable.Interact(Transform parent)
	{
		GrabbingAvailibility(parent);
	}


	private void GrabbingAvailibility(Transform parent)
	{
		if (isGrabbed && transform.parent == parent)
		{
			isGrabbed = !isGrabbed;
			transform.parent = null;
		}
		else if (!isGrabbed)
		{
			isGrabbed = !isGrabbed;
			//transform.position = parent.position;
			transform.parent = parent;
		}
	}
}
