using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BearTraper : TrapAbstract
{

	private PunSpawner punSpawner = null;

	[SerializeField] private GameObject bearTrapPrefab = null;
	private GameObject newBearTrap = null;
	private BearTrap BearTrapClas = null;


	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		float lifeTimeRate = punSpawner.CountDownTimer + 100;
		if (photonView.IsMine)
		{
			Destroy(this, lifeTimeRate);
		}
		bearTrapPrefab = Resources.Load("BearTrap", typeof(GameObject)) as GameObject;
	}

	public override void SetTrap()
	{
		newBearTrap = PhotonNetwork.Instantiate(bearTrapPrefab.name, transform.position, Quaternion.identity);
		BearTrapClas = newBearTrap.GetComponent<BearTrap>();
		BearTrapClas.SetTheTag(gameObject.tag);
	}
}
