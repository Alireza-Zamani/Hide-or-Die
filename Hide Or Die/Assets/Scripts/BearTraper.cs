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
		bearTrapPrefab = Resources.Load("BearTrap", typeof(GameObject)) as GameObject;
	}

	private void TrapBtnTurnOff()
	{
		trapSetBtn.SetActive(false);
	}

	public override void SetTrap()
	{
		newBearTrap = PhotonNetwork.Instantiate(bearTrapPrefab.name, transform.position, Quaternion.identity);
		BearTrapClas = newBearTrap.GetComponent<BearTrap>();
		BearTrapClas.SetTheTag(gameObject.tag);
	}
}
