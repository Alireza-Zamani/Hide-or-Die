using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilitySpear : AbilityAbstract
{

	[SerializeField] private GameObject spearPrefab = null;
	private float throwSpeed = 800f;
	private GameObject newSpear = null;

	private IPlayer playerInterface = null;

	private void Awake()
	{
		spearPrefab = Resources.Load("Spear", typeof(GameObject)) as GameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void ExecuteAbility(Vector2 aimingDirection)
	{
		newSpear = PhotonNetwork.Instantiate(spearPrefab.name, transform.position, Quaternion.identity);
		newSpear.GetComponent<Spear>().PlayerInterface = playerInterface;

		LookTowards(-aimingDirection , newSpear.transform);

		aimingDirection = aimingDirection.normalized * throwSpeed;
		newSpear.GetComponent<Rigidbody2D>().AddForce(aimingDirection, ForceMode2D.Force);
	}

	private void LookTowards(Vector2 dir , Transform spear)
	{
		// Rotate towards the joystick direction
		var dirAngular2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (dir.x < 0)
		{
			spear.rotation = Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}
		else
		{
			spear.rotation = Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}
	}
}