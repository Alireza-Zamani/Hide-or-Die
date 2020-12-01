using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtns : MonoBehaviour
{

	public delegate void OnDelegateChanged();
	public OnDelegateChanged onActionBtnDelegate;
	public OnDelegateChanged onAimingSelectDelegate;
	public OnDelegateChanged onAimingDeSelectDelegate;
	public OnDelegateChanged onShopBtnDelegate;
	public OnDelegateChanged onLockBtnDelegate;
	public OnDelegateChanged onSetTrapBtnDelegate;
	public OnDelegateChanged onWeaponBtnSelectDelegate;
	public OnDelegateChanged onWeaponBtnDeSelectDelegate;
	
	public OnDelegateChanged onWeaponSaberBuyBtnSelectDelegate;
	public OnDelegateChanged onWeaponMaceBuyBtnSelectDelegate;
	public OnDelegateChanged onWeaponKnifeBuyBtnSelectDelegate;
	public OnDelegateChanged onWeaponClubBtnSelectDelegate;
	public OnDelegateChanged onWeaponPistolBtnSelectDelegate;
	
	public OnDelegateChanged onWeaponMenuCloseBtnSelectDelegate;

	
	
	
	public void onWeaponMenuCloseBtn()
	{
		if(onWeaponMenuCloseBtnSelectDelegate != null)
		{
			onWeaponMenuCloseBtnSelectDelegate.Invoke();
		}
	}
	
	public void onWeaponPistolBtn()
	{
		if(onWeaponPistolBtnSelectDelegate != null)
		{
			onWeaponPistolBtnSelectDelegate.Invoke();
		}
	}
	
	public void onWeaponClubBtn()
	{
		if(onWeaponClubBtnSelectDelegate != null)
		{
			onWeaponClubBtnSelectDelegate.Invoke();
		}
	}
	
	public void onWeaponKnifeBuyBtn()
	{
		if(onWeaponKnifeBuyBtnSelectDelegate != null)
		{
			onWeaponKnifeBuyBtnSelectDelegate.Invoke();
		}
	}
	
	public void onWeaponMaceBuyBtn()
	{
		if(onWeaponMaceBuyBtnSelectDelegate != null)
		{
			onWeaponMaceBuyBtnSelectDelegate.Invoke();
		}
	}

	public void onWeaponSaberBuyBtn()
	{
		if(onWeaponSaberBuyBtnSelectDelegate != null)
		{
			onWeaponSaberBuyBtnSelectDelegate.Invoke();
		}
	}
	
	

	public void OnActionBtn()
	{
		if(onActionBtnDelegate != null)
		{
			onActionBtnDelegate.Invoke();
		}
	}

	public void OnWeaponSelect()
	{
		if (onWeaponBtnSelectDelegate != null)
		{
			onWeaponBtnSelectDelegate.Invoke();
		}
	}
	
	public void OnWeaponDeSelect()
	{
		if (onWeaponBtnDeSelectDelegate != null)
		{
			onWeaponBtnDeSelectDelegate.Invoke();
		}
	}
	

	public void OmAimingSelect()
	{
		if (onAimingSelectDelegate != null)
		{
			onAimingSelectDelegate.Invoke();
		}
	}

	public void OmAimingDeSelect()
	{
		if (onAimingDeSelectDelegate != null)
		{
			onAimingDeSelectDelegate.Invoke();
		}
	}


	public void OnShopBtn()
	{
		if (onShopBtnDelegate != null)
		{
			onShopBtnDelegate.Invoke();
		}
	}

	public void OnLockBtn()
	{
		if (onLockBtnDelegate != null)
		{
			onLockBtnDelegate.Invoke();
		}
	}


	public void OnSetTrapBtn()
	{
		if (onSetTrapBtnDelegate != null)
		{
			onSetTrapBtnDelegate.Invoke();
		}
	}


}
