using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private GameObject player = null;

	private void Update()
	{
		if(player != null)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y , transform.position.z);
		}
	}
}
