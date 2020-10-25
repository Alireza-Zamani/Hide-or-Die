using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class MovementAbstract : MonoBehaviourPunCallbacks
{

	#region Vars

	//Classes
	protected FloatingJoystick joystick = null;
	protected FieldOFView fielOfView = null;
	protected AnimatorController animatorController = null;

	//Components
	protected Rigidbody2D rb = null;

	//Fields
	[Range(0, 2000)] [SerializeField] protected float moveSpeed = 1000f;
	[Range(0, 50)] [SerializeField] protected float viewDistance = 10f;
	

	#endregion


	private void OnEnable()
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (fielOfView != null)
		{
			SetTheFOVSettings();
		}
	}

	public virtual void Awake()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		// Get the components from the Hierachy
		joystick = GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).GetComponent<FloatingJoystick>();
		fielOfView = GameObject.FindGameObjectWithTag("Fov").transform.GetComponent<FieldOFView>();
		animatorController = GetComponent<AnimatorController>();

		rb = GetComponent<Rigidbody2D>();
	}

	public virtual void Start()
	{
		if (!photonView.IsMine)
		{
			return;
		}

		SetTheFOVSettings();
		GameObject.FindGameObjectWithTag("CinemachineCamera").GetComponent<CameraFollow>().SettheFollowTarget(transform);
	}

	public virtual void SetTheFOVSettings()
	{
		fielOfView.ViewDistance = viewDistance;
	}

	public virtual void Update()
	{
		if (!photonView.IsMine)
		{
			return;
		}
		
		//Get the direction from joystick
		Vector2 direction = GetDirection();
		rb.AddForce(direction * moveSpeed * Time.deltaTime, ForceMode2D.Force);

		//Set the FOV vars
		fielOfView.SetTheOrigin(new Vector2(transform.position.x, transform.position.y));

		//Aniamtions
		if(animatorController != null)
		{
			if (direction != Vector2.zero)
			{
				animatorController.CanWalk();
			}
			else
			{
				animatorController.CanIdle();
			}
		}
	}

	public virtual Vector2 GetDirection()
	{
		//Horizontal Input
		float horizontal = joystick.Horizontal;
		//Flip ToRight
		if (horizontal >= 0.2f)
		{
			if (transform.localScale.x > 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = -5;
				transform.localScale = newScale;
			}
		}
		//Flip ToLeft
		else if (horizontal <= -0.2f)
		{
			if (transform.localScale.x < 0)
			{
				Vector2 newScale = transform.localScale;
				newScale.x = 5;
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
