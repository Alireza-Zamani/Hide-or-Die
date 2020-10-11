using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBodyHandler : MonoBehaviour
{


	public void ResetPlayer()
	{
		GameObject deadPlayer = gameObject.transform.GetChild(0).gameObject;
		deadPlayer.SetActive(true);
		deadPlayer.GetComponent<IPlayer>().Heal(100f);
		deadPlayer.transform.parent = null;
		Destroy(gameObject);
	}
}
