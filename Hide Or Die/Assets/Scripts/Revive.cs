using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Revive : MonoBehaviour
{
	[Range(0, 10)] [SerializeField] private float radiousOfAction = 5f;
	[SerializeField] private LayerMask raycastableForInSightLayerMask = new LayerMask();
	[SerializeField] private LayerMask revivableLayerMask = new LayerMask();

	private float timeCounter = 0f;

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	private PhotonView photonView;

	private string team = null;

	private void Start()
	{
		photonView = PhotonView.Get(this);
		if (!photonView.IsMine)
		{
			return;
		}

		team = playerInterface.TeamGetter();
		Reviving();
		PhotonNetwork.Destroy(gameObject);
	}

	private void Reviving()
	{
		photonView.RPC("RPCReviving", RpcTarget.AllBuffered);
	}

	[PunRPC]
	private void RPCReviving()
	{
		// Finds all players in the radious
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, revivableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			// Finds which one of these players are in the sight
			Vector2 dir = coll.collider.gameObject.transform.position - transform.position;
			float distance = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
			RaycastHit2D hittest = Physics2D.Raycast(transform.position, dir.normalized, distance, raycastableForInSightLayerMask);
			if (hittest.collider == null)
			{
				DeadBodyHandler deadBodyHandler = coll.collider.gameObject.GetComponent<DeadBodyHandler>();
				if (deadBodyHandler != null && coll.collider.gameObject.transform.GetChild(0).gameObject.tag == team)
				{
					deadBodyHandler.ResetPlayer();
					print("Is Reviving");
					return;
				}
			}
		}
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
