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

	private GameObject canvas = null;

	private MovementAbstract movementClass = null;

	private Text lockBtnText = null;

	[SerializeField] private GameObject aimingPrefab = null;

	private GameObject newAiming = null;

	private IInteractable interactable = null;

	private DoorInteractable doorInteractable = null;

	private AbilityAbstract ability = null;

	private TrapAbstract trapClass = null;

	private UIBtns uiBtns = null;

	private WeaponManager weaponManager;

	private GameObject weaponFixedJoyStick = null;

	private GameObject shopCanvas;

	private GameObject saberPrefab;
	private GameObject macePrefab;
	private GameObject clubPrefab;
	private GameObject knifePrefab;
	private GameObject pistolPrefab;

	private void OnDrawGizmos()
	{
		if (!doGizmos)
		{
			return;
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radiousOfAction);
	}

	private void Awake()
	{
		weaponFixedJoyStick = GameObject.FindGameObjectWithTag("UI").transform.GetChild(9).gameObject;
	}

	private void Start()
	{
		if (!photonView.IsMine)
		{
			return;
		}
		canvas = GameObject.FindGameObjectWithTag("UI").gameObject;
		movementClass = GetComponent<MovementAbstract>();
		ability = GetComponent<AbilityAbstract>();
		trapClass = GetComponent<TrapAbstract>();
		weaponManager = GetComponent<WeaponManager>();

		shopCanvas = GameObject.Find("Shop Canvas");
		shopCanvas = shopCanvas.transform.GetChild(0).gameObject;

		saberPrefab = Resources.Load<GameObject>("Melee Weapon Saber");
		//saberPrefab = Resources.Load<GameObject>("Objective");
		macePrefab = Resources.Load<GameObject>("Melee Weapon Mace");
		knifePrefab = Resources.Load<GameObject>("Melee Weapon Knife");
		clubPrefab = Resources.Load<GameObject>("Melee Weapon Club");
		pistolPrefab = Resources.Load<GameObject>("Gun Vintage Pistol");

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
		uiBtns.onSetTrapBtnDelegate += OnSetTrapBtn;
		uiBtns.onWeaponBtnSelectDelegate += OnWeaponSelect;
		uiBtns.onWeaponBtnDeSelectDelegate += OnWeaponDeSelect;

		uiBtns.onWeaponSaberBuyBtnSelectDelegate += OnSaberBuyBtn;
		uiBtns.onWeaponMaceBuyBtnSelectDelegate += OnMaceBuyBtn;
		uiBtns.onWeaponClubBtnSelectDelegate += OnClubBuyBtn;
		uiBtns.onWeaponKnifeBuyBtnSelectDelegate += OnKnifeBuyBtn;
		uiBtns.onWeaponPistolBtnSelectDelegate += OnPistolBuyBtn;
		

		uiBtns.onWeaponMenuCloseBtnSelectDelegate += OnCloseShopBtn;

	}

	public void RemoveDelegates()
	{
		uiBtns.onActionBtnDelegate -= OnActionBtn;
		uiBtns.onAimingSelectDelegate -= OnAimingSelect;
		uiBtns.onAimingDeSelectDelegate -= OnAimingDeSelect;
		uiBtns.onShopBtnDelegate -= OnShopBtn;
		uiBtns.onLockBtnDelegate -= OnLockBtn;
		uiBtns.onSetTrapBtnDelegate -= OnSetTrapBtn;
		uiBtns.onWeaponBtnSelectDelegate -= OnWeaponSelect;
		uiBtns.onWeaponBtnDeSelectDelegate -= OnWeaponDeSelect;
		
		uiBtns.onWeaponSaberBuyBtnSelectDelegate -= OnSaberBuyBtn;
		uiBtns.onWeaponMaceBuyBtnSelectDelegate -= OnMaceBuyBtn;
		uiBtns.onWeaponClubBtnSelectDelegate -= OnClubBuyBtn;
		uiBtns.onWeaponKnifeBuyBtnSelectDelegate -= OnKnifeBuyBtn;
		uiBtns.onWeaponPistolBtnSelectDelegate -= OnPistolBuyBtn;
		
		
		uiBtns.onWeaponMenuCloseBtnSelectDelegate -= OnCloseShopBtn;
	}

	private void OnSaberBuyBtn()
	{
		weaponManager.AddNewWeapon(saberPrefab);
		shopCanvas.SetActive(false);
	}
	
	private void OnMaceBuyBtn()
	{
		weaponManager.AddNewWeapon(macePrefab);
		shopCanvas.SetActive(false);
	}
	
	private void OnClubBuyBtn()
	{
		weaponManager.AddNewWeapon(clubPrefab);
		shopCanvas.SetActive(false);
	}
	
	private void OnKnifeBuyBtn()
	{
		weaponManager.AddNewWeapon(knifePrefab);
		shopCanvas.SetActive(false);
	}
	
	private void OnPistolBuyBtn()
	{
		weaponManager.AddNewWeapon(pistolPrefab);
		shopCanvas.SetActive(false);
	}
		
	private void OnWeaponSelect()
	{
		if (!weaponManager.currentWeapon)
		{
			weaponManager.GetChildedWeapon(this.gameObject);

			if (!weaponManager.currentWeapon)
				return;
		}

		if (weaponManager.currentWeaponType == WeaponAbstract.WeaponTypes.Gun)
		{
			movementClass.enabled = false;
			weaponManager.currentWeapon.Aim();
		}
		else
		{
			if (weaponFixedJoyStick.activeInHierarchy)
			{
				weaponFixedJoyStick.SetActive(false);
			}
			weaponManager.currentWeapon.Attack();
		}
		
		
	}
	
	private void OnWeaponDeSelect()
	{
		if (weaponManager.currentWeaponType == WeaponAbstract.WeaponTypes.Gun)
		{
			weaponManager.currentWeapon.Attack();
			movementClass.enabled = true;
		}
	}
	
	private void OnActionBtn()
	{
		photonView.RPC("Interact", RpcTarget.AllBuffered);
	}

	private void OnAimingSelect()
	{
		CallAbilityIsStarting();
		//newAiming = Instantiate(aimingPrefab, transform.position, Quaternion.identity);
		movementClass.enabled = false;
	}

	private void OnAimingDeSelect()
	{
		//if(newAiming != null)
		//{
		//	Destroy(newAiming);
		//}
		CallExecuteAbility();
		movementClass.enabled = true;
	}

	private void OnShopBtn()
	{
		shopCanvas.SetActive(true);
	}

	private void OnCloseShopBtn()
	{
		shopCanvas.SetActive(false);
	}


	private void OnLockBtn()
	{
		if (GetComponent<CollisionHandler>().HasLock == 0)
		{
			GetComponent<CollisionHandler>().UpdateLockBtn(false , null);
			return;
		}
		// Lock The Door
		photonView.RPC("Lock", RpcTarget.AllBuffered);
		print("Door Locked");
	}


	private void OnSetTrapBtn()
	{
		// Set the mine
		if(trapClass == null)
		{
			trapClass = GetComponent<TrapAbstract>();
		}
		if (trapClass != null)
		{
			trapClass.SetTrap();
			canvas.transform.GetChild(6).gameObject.SetActive(false);
			Destroy(trapClass);
		}
	}


	private void CallAbilityIsStarting()
	{
		if (ability != null)
		{
			ability.AbilityIsStarting(aimingPrefab);
		}
		else
		{
			ability = GetComponent<AbilityAbstract>();
			if (ability != null)
			{
				ability.AbilityIsStarting(aimingPrefab);
			}
			else
			{
				Debug.LogError("No ability attached to the player");
				return;
			}
			Debug.LogWarning("No ability attached to the player");
		}
	}

	private void CallExecuteAbility()
	{
		if(ability != null)
		{
			//Vector2 aimDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
			//ability.ExecuteAbility(-aimDirection);
			ability.ExecuteAbility();
		}
		else
		{
			ability = GetComponent<AbilityAbstract>();
			if(ability != null)
			{
				//Vector2 aimDirection = newAiming.GetComponent<AimingDirection>().AimDirection;
				//ability.ExecuteAbility(-aimDirection);
				ability.ExecuteAbility();
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
					if (photonView.IsMine)
					{
						GetComponent<CollisionHandler>().HasLock--;
					}
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
			}
		}

		if(interactable != null)
		{
			interactable.Interact(transform);
			interactable = null;
		}
	}

}
