using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPunCallbacks
{

	#region Vars

	//Classes
	private FloatingJoystick joystick = null;
	private FieldOFView fielOfView = null;
	private AnimatorController animatorController = null;

	//Components
	private Rigidbody2D rb = null;

	//Fields
	[SerializeField] private float moveSpeed = 200f;

	#endregion


	private void Awake()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		// Change the layer to Player so that we wont be our own enemy because at the first all of the layers are at Enemy
		gameObject.layer = LayerMask.NameToLayer("Player");

		// Get the components from the Hierachy
		joystick = GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).GetComponent<FloatingJoystick>();
		fielOfView = GameObject.FindGameObjectWithTag("Fov").transform.GetComponent<FieldOFView>();
		animatorController = GetComponent<AnimatorController>();

		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().Player = gameObject;
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		//Get the direction from joystick
		Vector2 direction = GetDirection();
		rb.AddForce(direction * moveSpeed * Time.deltaTime, ForceMode2D.Force);

		//Set the FOV vars
		fielOfView.SetTheOrigin(new Vector2(transform.position.x , transform.position.y));

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
