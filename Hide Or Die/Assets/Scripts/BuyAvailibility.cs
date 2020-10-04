using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyAvailibility : MonoBehaviour
{
	private bool canBuy = true;
	public bool CanBuy { get => canBuy; set { canBuy = value; ChangeBuyBtn(value); } }

	private GameObject buyBtn = null;

	private void Start()
	{
		buyBtn = GameObject.FindGameObjectWithTag("UI").transform.GetChild(3).transform.gameObject;
	}

	private void ChangeBuyBtn(bool activity)
	{
		if (buyBtn.activeInHierarchy)
		{
			buyBtn.SetActive(activity);
		}
	}
}
