using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

	#region Vars

	//Classes
	[SerializeField] private DynamicJoystick joystick = null;
	private AnimatorController animatorController;

	//Components
	private Rigidbody2D rb;

	//Fields
	[SerializeField] private float moveSpeed = 200f;

	#endregion


	private void Awake()
	{
		animatorController = GetComponent<AnimatorController>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		Vector2 direction = GetDirection();
		rb.AddForce(direction * moveSpeed * Time.deltaTime, ForceMode2D.Force);

		//Aniamtions
		if (direction != Vector2.zero)
		{
			animatorController.CanWalk();
		}
		else
		{
			animatorController.CanIdle();
		}
	}

	private Vector2 GetDirection()
	{
		//Horizontal Input
		float horizontal = joystick.Horizontal;
		//Flip ToRight
		if(horizontal >= 0.2f)
		{
			if (transform.localScale.x > 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = -1;
				transform.localScale = newScale;
			}
		}
		//Flip ToLeft
		else if (horizontal <= -0.2f)
		{
			if (transform.localScale.x < 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = 1;
				transform.localScale = newScale;
			}
		}
		else
		{
			horizontal = 0;
		}

		//Vertical Input
		float vertical = joystick.Vertical;
		if (vertical < 0.2f && vertical > -0.2f)
		{
			vertical = 0;
		}

		Vector2 dir = new Vector2(horizontal, vertical);
		return dir.normalized;
	}
}
