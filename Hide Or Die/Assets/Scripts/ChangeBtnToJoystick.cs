using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ChangeBtnToJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	private enum btnFunctionality { Ability , Weapon}

	[SerializeField] private btnFunctionality function = btnFunctionality.Ability;

	[SerializeField] private bool isAbilityBtn = true;

	[SerializeField] private GameObject joystick = null;

	[SerializeField] private Image shoot = null;


	[SerializeField] private UIBtns uIBtns = null;


	public void OnDrag(PointerEventData eventData)
	{
		if (joystick.activeInHierarchy)
		{
			joystick.GetComponent<FixedJoystick>().OnDrag(eventData);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		joystick.GetComponent<FixedJoystick>().OnPointerDown(eventData);
		shoot.enabled = false;
		joystick.SetActive(true);
		switch (function)
		{
			case btnFunctionality.Ability:
				uIBtns.onAimingSelectDelegate();
				break;
			case btnFunctionality.Weapon:
				uIBtns.onWeaponBtnSelectDelegate();
				break;
		}
		
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		joystick.GetComponent<FixedJoystick>().OnPointerUp(eventData);
		shoot.enabled = true;
		joystick.SetActive(false);
		switch (function)
		{
			case btnFunctionality.Ability:
				uIBtns.onAimingDeSelectDelegate();
				break;
			case btnFunctionality.Weapon:
				uIBtns.onWeaponBtnDeSelectDelegate();
				break;
		}
		
	}
}
