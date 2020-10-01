using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDirection : MonoBehaviour
{

	private Vector2 aimDirection = Vector2.zero;
	public Vector2 AimDirection { get => aimDirection; set => aimDirection = value; }

	private FloatingJoystick joystick = null;
	private bool joystickChanged = false;


	private Vector2 dir = Vector2.zero;

	public virtual void Awake()
	{
		joystick = GameObject.FindGameObjectWithTag("UI").transform.GetChild(0).GetComponent<FloatingJoystick>();
	}

	private void Start()
	{
		Vector3 pos = transform.position;
		pos.z = 100;
		transform.position = pos;
	}

	private void Update()
	{
		dir = -GetDirection();
		
		// IF its the first time we are changing joystick then we have to change bool
		if (!joystickChanged && dir != Vector2.zero)
		{
			joystickChanged = true;
		}
		// If we have changed joystick before but now the joystick direction is zero then we shouldnt rotate the aimer
		else if(joystickChanged && dir == Vector2.zero)
		{
			return;
		}

		// Rotate towards the joystick direction
		var dirAngular2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (dir.x < 0)
		{
			transform.rotation = Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}
		else
		{
			transform.rotation = Quaternion.AngleAxis(dirAngular2, Vector3.forward);
		}

		AimDirection = dir;
	}

	public virtual Vector2 GetDirection()
	{
		//Horizontal Input
		float horizontal = joystick.Horizontal;
		

		//Vertical Input
		float vertical = joystick.Vertical;

		Vector2 dir = new Vector2(horizontal, vertical);
		return dir.normalized;
	}

}
