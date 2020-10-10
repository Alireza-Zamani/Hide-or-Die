using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilityRevive : AbilityAbstract
{
	[SerializeField] private GameObject revivePrefab = null;

	private GameObject newRevive = null;

	private IPlayer playerInterface = null;

	private void Awake()
	{
		revivePrefab = Resources.Load("Revive", typeof(GameObject)) as GameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void ExecuteAbility(Vector2 aimingDirection)
	{
		newRevive = PhotonNetwork.Instantiate(revivePrefab.name, transform.position, Quaternion.identity);
		newRevive.GetComponent<Revive>().PlayerInterface = playerInterface;
	}
}
