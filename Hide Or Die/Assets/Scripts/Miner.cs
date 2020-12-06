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
	private GameObject trapSetBtn = null;


	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		trapSetBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(6).gameObject;
		float lifeTimeRate = punSpawner.CountDownTimerForTrapUsebality;
		if (photonView.IsMine)
		{
			Invoke("TrapBtnTurnOff", lifeTimeRate);
			Destroy(this , lifeTimeRate);
		}
		minePrefab = Resources.Load("Mine", typeof(GameObject)) as GameObject;
	}


	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}

	public override void SetTrap()
	{
		newMine = PhotonNetwork.Instantiate(minePrefab.name, transform.position, Quaternion.identity);
		mineClas = newMine.GetComponent<Mine>();
		mineClas.SetTheTag(gameObject.tag);
	}
}
