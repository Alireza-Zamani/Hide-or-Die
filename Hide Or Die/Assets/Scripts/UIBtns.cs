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



	public void OnActionBtn()
	{
		onActionBtnDelegate.Invoke();
	}


	public void OmAimingSelect()
	{
		onAimingSelectDelegate.Invoke();
	}

	public void OmAimingDeSelect()
	{
		onAimingDeSelectDelegate.Invoke();
	}


	public void ShopBtn()
	{
		onShopBtnDelegate.Invoke();
	}

}
