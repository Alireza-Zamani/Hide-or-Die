using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrapObjectiveInteractability : MonoBehaviourPunCallbacks, IInteractable
{

	private PunSpawner punSpawner = null;

	private string trapClassName = null;
	public string TrapClassName { get => trapClassName; set => photonView.RPC("RPCTrapClassName" , RpcTarget.AllBuffered , value); }

	private void Start()
	{
		punSpawner = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PunSpawner>();
		
		float lifeTimeRate = punSpawner.CountDownTimer + 100f;
		if (TrapClassName == "TrapDetectorBeeper")
		{
			lifeTimeRate += 15f;
		}
		if (photonView.IsMine)
		{
			Invoke("DestroyGameObject", lifeTimeRate);
		}
		
	}

	void IInteractable.Interact(Transform parent)
	{
		IPlayer iplayer = parent.gameObject.GetComponent<IPlayer>();
		if(iplayer != null)
		{
			if (!iplayer.HasTrapGetter())
			{
				iplayer.HasTrapSetter(true);

				iplayer.AddComponent(TrapClassName);

				if (photonView.IsMine)
				{
					DestroyGameObject();
				}
			}
		}

	}

	private void DestroyGameObject()
	{
		PhotonNetwork.Destroy(gameObject);
	}

	[PunRPC]
	private void RPCTrapClassName(string trapClassName)
	{
		this.trapClassName = trapClassName;
	}
}
