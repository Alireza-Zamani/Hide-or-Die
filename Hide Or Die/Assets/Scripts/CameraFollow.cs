using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private GameObject player = null;
	public GameObject Player { get => player; set => player = value; }

	private void Update()
	{
		if(player != null)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y , transform.position.z);
		}
	}
}
