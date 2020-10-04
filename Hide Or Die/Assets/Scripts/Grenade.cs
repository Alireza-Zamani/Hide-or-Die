using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviour
{

	[Range(0 , 10)] [SerializeField] private float explosionTimeCount = 5f;
	[Range(0, 100)] [SerializeField] private float explosionDamage = 50f;
	[Range(0, 10)] [SerializeField] private float radiousOfAction = 5f;
	[SerializeField] private LayerMask damagableLayerMask = new LayerMask();

	private IPlayer playerInterface = null;

	private void Start()
	{
		Invoke("Explode", explosionTimeCount);
	}

	private void Explode()
	{
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, damagableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<IPlayer>() != null)
			{
				playerInterface = coll.collider.gameObject.GetComponent<IPlayer>();
				print("Is Damaging");
			}
		}

		PhotonNetwork.Destroy(gameObject);
	}

}
