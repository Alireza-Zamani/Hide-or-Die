using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class Action : MonoBehaviourPunCallbacks
{

	[SerializeField] private bool doGizmos = false;
	[Range(0, 5)] [SerializeField] private float radiousOfAction = 0f;
	[SerializeField] private LayerMask actionableLayerMask = new LayerMask();

	private MovementAbstract movementClass = null;

	[SerializeField] private GameObject aimingPrefab = null;

	private GameObject newAiming = null;

	private IInteractable interactable = null;



	private void OnDrawGizmos()
	{
		if (!doGizmos)
		{
			return;
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radiousOfAction);
	}

	private void Start()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		movementClass = GetComponent<MovementAbstract>();

		//Set the action button listener
		UIBtns uiBtns = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>();
		uiBtns.onActionBtnDelegate = OnActionBtn;
		uiBtns.onAimingSelectDelegate = OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate = OnAimingDeSelect;
	}


	private void OnActionBtn()
	{
		photonView.RPC("Interact", RpcTarget.AllBuffered);
	}

	private void OnAimingSelect()
	{
		newAiming = Instantiate(aimingPrefab, transform.position, Quaternion.identity);
		movementClass.enabled = false;
	}

	private void OnAimingDeSelect()
	{
		Destroy(newAiming);
		movementClass.enabled = true;
	}

	[PunRPC]
	public void Interact()
	{
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, actionableLayerMask);
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<IInteractable>() != null)
			{
				interactable = coll.collider.gameObject.GetComponent<IInteractable>();
				break;
			}
		}
		interactable.Interact(transform);
	}

}
