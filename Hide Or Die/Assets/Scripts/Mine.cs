using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPunCallbacks
{

	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;

	[SerializeField] private GameObject explosionEffectPrefab = null;



	private void Start()
	{
		if (!photonView.IsMine)
		{

			// If the mine is in the the enemy game then turn off the sprite renderer
			if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
			{
				Debug.LogError("No hashTable exists for team");
			}

			int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
			if((gameObject.tag == "BlueTeam" && team == 2) || (gameObject.tag == "RedTeam" && team == 1))
			{
				GetComponent<SpriteRenderer>().enabled = false;
			}
			Destroy(this);
			return;
		}
	}

	public void SetTheTag(string team)
	{
		photonView.RPC("RPCSetTheTag", RpcTarget.AllBuffered, team);
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
				// If the other was in the other team and was a player (BlueTeam -- RedTeam) then explode the mine
				other.gameObject.GetComponent<IPlayer>().TakeDamage(explosionDamage);
				PhotonNetwork.Instantiate(explosionEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, explosionEffectPrefab.transform.position.z) , Quaternion.identity);
				DestroyGameObject();
			}
		}
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}


	[PunRPC]
	private void RPCSetTheTag(string team)
	{
		gameObject.tag = team;
	}
}
