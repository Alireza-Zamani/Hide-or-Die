using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spear : MonoBehaviourPunCallbacks
{

	private IPlayer playerInterface = null;
	public IPlayer PlayerInterface { get => playerInterface; set => playerInterface = value; }

	[Range(0, 5000)] [SerializeField] private float throwSpeed = 2500f;


	[Range(0, 100)] [SerializeField] private float hitDamage = 50f;

	private Vector2 aimingDirection = Vector2.zero;
	public Vector2 AimingDirection { get => aimingDirection; set => aimingDirection = value; }

	private Rigidbody2D rb = null;

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
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		rb.AddForce(AimingDirection * throwSpeed * Time.deltaTime, ForceMode2D.Force);
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
