using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Poisoner : TrapAbstract
{

	private PunSpawner punSpawner = null;

	[SerializeField] private GameObject poisonPrefab = null;
	private GameObject newPoison = null;
	private Poison poisonClas = null;

	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		float lifeTimeRate = punSpawner.CountDownTimer + 100;
		if (photonView.IsMine)
		{
			Destroy(this, lifeTimeRate);
		}
		poisonPrefab = Resources.Load("Poison", typeof(GameObject)) as GameObject;
	}


	public override void SetTrap()
	{
		newPoison = PhotonNetwork.Instantiate(poisonPrefab.name, transform.position, Quaternion.identity);
		poisonClas = newPoison.GetComponent<Poison>();
		poisonClas.SetTheTag(gameObject.tag);
	}
}
