using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BearTrap : MonoBehaviourPunCallbacks , ITrap
{
	[Range(0, 100)] [SerializeField] private float bearTrapDamage = 30f;
	[Range(0, 100)] [SerializeField] private float bearTrapLifeTimeRate = 5f;
	[Range(0, 100)] [SerializeField] private float bearTrapStuckTimeRate = 5f;

	[SerializeField] private GameObject bearTrapEffectPrefab = null;

	private bool isExecuted = false;



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
			if ((gameObject.tag == "BlueTeam" && team == 2) || (gameObject.tag == "RedTeam" && team == 1))
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
			if (other.tag == "BlueTeam" || other.tag == "RedTeam" && !isExecuted)
			{
				// If the other was in the other team and was a player (BlueTeam -- RedTeam) then explode the mine
				other.gameObject.GetComponent<IPlayer>().TakeDamage(bearTrapDamage);
				other.gameObject.GetComponent<IPlayer>().StuckPlayer(bearTrapStuckTimeRate);
				PhotonNetwork.Instantiate(bearTrapEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, bearTrapEffectPrefab.transform.position.z), Quaternion.identity);
				isExecuted = true;
				Invoke("DestroyGameObject", bearTrapLifeTimeRate);
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
