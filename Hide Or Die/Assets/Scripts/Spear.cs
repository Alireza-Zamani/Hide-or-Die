using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spear : MonoBehaviourPunCallbacks
{

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	[Range(0, 100)] [SerializeField] private float hitDamage = 50f;

	private string team = null;

	private void Start()
	{
		if (!photonView.IsMine)
		{
			Destroy(this);
			return;
		}

		Invoke("DestroyGameObject", 5f);
		team = playerInterface.TeamGetter();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (other.tag != team)
		{
			if(other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				print(other.gameObject.name);
				other.gameObject.GetComponent<IPlayer>().TakeDamage(hitDamage);
				DestroyGameObject();
			}
			else if(other.tag == "Environment")
			{
				DestroyGameObject();
			}
		}
	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}

}
