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

	private Text lockBtnText = null;

	[SerializeField] private GameObject aimingPrefab = null;

	private GameObject newAiming = null;

	private IInteractable interactable = null;

	private DoorInteractable doorInteractable = null;

	private AbilityAbstract ability = null;

	private Miner minerClass = null;

	private UIBtns uiBtns = null;



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
		ability = GetComponent<AbilityAbstract>();
		minerClass = GetComponent<Miner>();

		//Set the action button listener
		uiBtns = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBtns>();
		AddDelegates();
	}


	public void AddDelegates()
	{
		uiBtns.onActionBtnDelegate += OnActionBtn;
		uiBtns.onAimingSelectDelegate += OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate += OnAimingDeSelect;
		uiBtns.onShopBtnDelegate += OnShopBtn;
		uiBtns.onLockBtnDelegate += OnLockBtn;
		uiBtns.onMineBtnDelegate += OnMineBtn;
	}

	public void RemoveDelegates()
	{
		uiBtns.onActionBtnDelegate -= OnActionBtn;
		uiBtns.onAimingSelectDelegate -= OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate -= OnAimingDeSelect;
		uiBtns.onShopBtnDelegate -= OnShopBtn;
		uiBtns.onLockBtnDelegate -= OnLockBtn;
		uiBtns.onMineBtnDelegate -= OnMineBtn;
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
		CallExecuteAbility();
		movementClass.enabled = true;
	}

	private void OnShopBtn()
	{
		// Open the shop menue
		print("Shop Menue Opened");
	}


	private void OnLockBtn()
	{
		// Lock The Door
		photonView.RPC("Lock", RpcTarget.AllBuffered);
		print("Door Locked");
	}


	private void OnMineBtn()
	{
		// Set the mine
		minerClass.SetMine();
		print("Mine Setted");
	}
	private void CallExecuteAbility()
	{
		if(ability != null)
		{
			Vector2 aimDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
			ability.ExecuteAbility(-aimDirection);
		}
		else
		{
			ability = GetComponent<AbilityAbstract>();
			if(ability != null)
			{
				Vector2 aimDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
				ability.ExecuteAbility(-aimDirection);
			}
			else
			{
				Debug.LogError("No ability attached to the player");
				return;
			}
			Debug.LogWarning("No ability attached to the player");
		}
	}

	[PunRPC]
	public void Lock()
	{
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, actionableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<DoorInteractable>() != null)
			{
				if (Vector2.Distance(coll.collider.gameObject.transform.position, transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
			if (newInteractable != null)
			{
				doorInteractable = newInteractable.GetComponent<DoorInteractable>();
			}
		}

		if (doorInteractable != null)
		{
			lockBtnText = GameObject.FindGameObjectWithTag("UI").transform.GetChild(5).transform.GetChild(0).gameObject.GetComponent<Text>();
			if (lockBtnText.text == "Lock")
			{
				if (doorInteractable.LockDoor(true))
				{
					lockBtnText.text = "UnLock";
				}
			}
			else if(lockBtnText.text == "UnLock")
			{
				if (doorInteractable.LockDoor(false))
				{
					lockBtnText.text = "Lock";
				}
				
			}
			doorInteractable = null;
		}
	}

	[PunRPC]
	public void Interact()
	{
		RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, radiousOfAction, Vector2.up, 10, actionableLayerMask);
		float dist = Mathf.Infinity;
		GameObject newInteractable = null;
		foreach (RaycastHit2D coll in hit)
		{
			if (coll.collider.gameObject.GetComponent<IInteractable>() != null)
			{
				if(Vector2.Distance(coll.collider.gameObject.transform.position , transform.position) < dist)
				{
					dist = Vector2.Distance(coll.collider.gameObject.transform.position, transform.position);
					newInteractable = coll.collider.gameObject;
				}
			}
			if(newInteractable != null)
			{
				interactable = newInteractable.GetComponent<IInteractable>();
				print(coll.collider.gameObject.name);
			}
		}

		if(interactable != null)
		{
			interactable.Interact(transform);
			interactable = null;
		}
	}

}
