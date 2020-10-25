using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Poison : MonoBehaviourPunCallbacks , ITrap
{
	[Range(0, 100)] [SerializeField] private float poisingDamage = 10f;
	[Range(0, 100)] [SerializeField] private float poisonEffectRate = 1f;
	[Range(0, 100)] [SerializeField] private float poisonLifeTimeRate = 3f;
	private float timer = 0f;
	[SerializeField] private GameObject poisonEffectPrefab = null;
	private List<GameObject> inRangeEnemies = new List<GameObject>();
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

	private void Update()
	{
		if (isExecuted)
		{
			timer += Time.deltaTime;
			if(timer >= 1)
			{
				timer = 0f;
				foreach(GameObject go in inRangeEnemies)
				{
					go.GetComponent<IPlayer>().TakeDamage(poisingDamage);
				}
			}
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
				// If the other game object was in the other team and was a player (BlueTeam -- RedTeam) then add it to the enemies
				if (!inRangeEnemies.Contains(other.gameObject))
				{
					inRangeEnemies.Add(other.gameObject);
				}
				if (!isExecuted)
				{
					ExecutePoisonDamage();
					isExecuted = true;
				}
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (other.tag != gameObject.tag)
		{
			if (other.tag == "BlueTeam" || other.tag == "RedTeam")
			{
				// If the other game object was in the other team and was a player (BlueTeam -- RedTeam) then if it was in the ranged enemies remove it
				if (inRangeEnemies.Contains(other.gameObject))
				{
					inRangeEnemies.Remove(other.gameObject);
				}
				
			}
		}
	}

	private void ExecutePoisonDamage()
	{
		PhotonNetwork.Instantiate(poisonEffectPrefab.name, new Vector3(transform.position.x, transform.position.y, poisonEffectPrefab.transform.position.z), Quaternion.identity);
		Invoke("DestroyGameObject", poisonLifeTimeRate);
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
