using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMatchData : MonoBehaviour , IPlayer
{

	[SerializeField] private GameObject bodyHandler = null;

	[SerializeField] private float health = 100;

	public float Health { get => health; set => health = value; }

	private string team = null;

	private string className = null;

	private PhotonView photonView;

	private void Awake()
	{
		AddAbility();
	}


	private void Start()
	{
		photonView = PhotonView.Get(this);
	}

	public void AddAbility()
	{
		if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
		{
			Debug.LogError("No hashTable exists for team");
		}

		className = (string)PhotonNetwork.LocalPlayer.CustomProperties["Class"];

		switch (className)
		{
			case "Healer":
				gameObject.AddComponent<AbilityHealer>();
				break;
			case "Spearer":
				gameObject.AddComponent<AbilitySpear>();
				break;
			case "Grenader":
				gameObject.AddComponent<AbilityGrenade>();
				break;
			case "Teleporter":
				gameObject.AddComponent<AbilityTeleport>();
				break;
			case "Reviver":
				gameObject.AddComponent<AbilityRevive>();
				break;
			case "Thief":
				gameObject.AddComponent<AbilityHealer>();
				break;
			case "Guard":
				gameObject.AddComponent<AbilityHealer>();
				break;

			// If no ability was choosed we will give the player grenade ability automaticlly
			default:
				gameObject.AddComponent<AbilityGrenade>();
				break;
		}
	}

	public string TeamGetter()
	{
		return team;
	}

	public void TeamSetter(string team)
	{
		this.team = team;
	}

	public void TakeDamage(float damageAmount)
	{
		photonView.RPC("RPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
	}

	[PunRPC]
	public void RPCTakeDamage(float damageAmount)
	{
		Health -= damageAmount;

		if (Health <= 0)
		{
			Die();
		}
	}

	public void Die()
	{
		print("Player " + gameObject.name + " Died");
		GameObject newBodyHandler = Instantiate(bodyHandler, transform.position, Quaternion.identity);
		transform.parent = newBodyHandler.transform;
		gameObject.SetActive(false);
	}

	public void Heal(float healAmount)
	{
		photonView.RPC("RPCHeal", RpcTarget.AllBuffered, healAmount);
	}

	[PunRPC]
	public void RPCHeal(float damageAmount)
	{
		Health += damageAmount;
		Health = Mathf.Clamp(Health , 0 , 100);
	}
}
