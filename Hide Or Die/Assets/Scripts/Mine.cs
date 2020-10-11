using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPunCallbacks
{

	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;

	[SerializeField] private GameObject explosionEffectPrefab = null;

	[SerializeField] private AudioClip explosionSoundEffect = null;
	private AudioSource audioSource = null;


	private void Start()
	{
		if (!photonView.IsMine)
		{
			Destroy(this);
			return;
		}
		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
	}

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
				other.gameObject.GetComponent<IPlayer>().TakeDamage(explosionDamage);
				PhotonNetwork.Instantiate(explosionEffectPrefab.name, transform.position, Quaternion.identity);
				audioSource.PlayOneShot(explosionSoundEffect);
				DestroyGameObject();
			}
		}
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
