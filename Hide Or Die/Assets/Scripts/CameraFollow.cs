using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{


	public void SettheFollowTarget(Transform player)
	{
		GetComponent<CinemachineVirtualCamera>().Follow = player;
	}
}
