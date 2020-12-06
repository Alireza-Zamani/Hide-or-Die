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
	private GameObject trapSetBtn = null;


	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		trapSetBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(6).gameObject;

		float lifeTimeRate = punSpawner.CountDownTimerForTrapUsebality;
		if (photonView.IsMine)
		{
			Invoke("TrapBtnTurnOff", lifeTimeRate);
			Destroy(this, lifeTimeRate);
		}
		poisonPrefab = Resources.Load("Poison", typeof(GameObject)) as GameObject;
	}

	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}


	public override void SetTrap()
	{
		newPoison = PhotonNetwork.Instantiate(poisonPrefab.name, transform.position, Quaternion.identity);
		poisonClas = newPoison.GetComponent<Poison>();
		poisonClas.SetTheTag(gameObject.tag);
	}
}
