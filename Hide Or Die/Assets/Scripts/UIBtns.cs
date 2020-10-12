using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBtns : MonoBehaviour
{

	public delegate void OnDelegateChanged();
	public OnDelegateChanged onActionBtnDelegate;
	public OnDelegateChanged onAimingSelectDelegate;
	public OnDelegateChanged onAimingDeSelectDelegate;
	public OnDelegateChanged onShopBtnDelegate;
	public OnDelegateChanged onLockBtnDelegate;
	public OnDelegateChanged onMineBtnDelegate;


	public void OnActionBtn()
	{
		if(onActionBtnDelegate != null)
		{
			onActionBtnDelegate.Invoke();
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


	public void OnMineBtn()
	{
		if (onMineBtnDelegate != null)
		{
			onMineBtnDelegate.Invoke();
		}
	}

}
