using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : TrapAbstract
{

	private PunSpawner punSpawner = null;

	[SerializeField] private GameObject minePrefab = null;
	private GameObject newMine = null;
	private Mine mineClas = null;


	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		float lifeTimeRate = punSpawner.CountDownTimer + 100;
		if (photonView.IsMine)
		{
			Destroy(this , lifeTimeRate);
		}
		minePrefab = Resources.Load("Mine", typeof(GameObject)) as GameObject;
	}

	public override void SetTrap()
	{
		print("Setted");
		newMine = PhotonNetwork.Instantiate(minePrefab.name, transform.position, Quaternion.identity);
		mineClas = newMine.GetComponent<Mine>();
		mineClas.SetTheTag(gameObject.tag);
	}
}
