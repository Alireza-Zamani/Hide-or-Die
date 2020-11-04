using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{

	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public void CanWalk()
	{
		if (!anim.GetBool("walk"))
		{
			anim.SetBool("walk", true);
		}
	}

	public void CanIdle()
	{
		if (anim.GetBool("walk"))
		{
			anim.SetBool("walk", false);
		}
	}

}
