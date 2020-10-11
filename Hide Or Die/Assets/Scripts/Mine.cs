using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPunCallbacks
{

	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;




	public void SetTheTag(string team)
	{
		photonView.RPC("RPCSetTheTag", RpcTarget.AllBuffered, team);
	}

	[PunRPC]
	private void RPCSetTheTag(string team)
	{
		gameObject.tag = team;
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (other.tag != gameObject.tag)
		{
			if (other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				print(other.gameObject.name);
				other.gameObject.GetComponent<IPlayer>().TakeDamage(explosionDamage);
				DestroyGameObject();
			}
		}
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
