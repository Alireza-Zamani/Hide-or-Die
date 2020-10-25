using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AbilitySpear : AbilityAbstract
{

	[SerializeField] private GameObject spearPrefab = null;
	private GameObject newSpear = null;

	private IPlayer playerInterface = null;

	GameObject newAiming = null;

	private void Awake()
	{
		spearPrefab = Resources.Load("Spear", typeof(GameObject)) as GameObject;
	}

	private void Start()
	{
		playerInterface = GetComponent<IPlayer>();
	}

	public override void AbilityIsStarting(GameObject aimingPref)
	{
		newAiming = Instantiate(aimingPref, transform.position, Quaternion.identity);
	}


	public override void ExecuteAbility()
	{
		Vector2 aimingDirection = Vector2.zero;
		if (newAiming != null)
		{
			aimingDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
			Destroy(newAiming);
		}

		aimingDirection = -aimingDirection;

		newSpear = PhotonNetwork.Instantiate(spearPrefab.name, transform.position, Quaternion.identity);
		Spear spearClass = newSpear.GetComponent<Spear>();
		spearClass.PlayerInterface = playerInterface;

		LookTowards(-aimingDirection , newSpear.transform);

		aimingDirection = aimingDirection.normalized;
		spearClass.AimingDirection = aimingDirection;
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