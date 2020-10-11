using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviourPunCallbacks
{

	[SerializeField] private GameObject minePrefab = null;
	private GameObject newMine = null;
	private Mine mineClas = null;

    public void SetMine()
	{
		newMine = PhotonNetwork.Instantiate(minePrefab.name, transform.position, Quaternion.identity);
		mineClas = newMine.GetComponent<Mine>();
		mineClas.SetTheTag(gameObject.tag);
	}

	

}
