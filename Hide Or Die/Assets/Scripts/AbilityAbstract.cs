using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class AbilityAbstract : MonoBehaviourPunCallbacks
{

	public virtual void ThrowGrenade(Vector2 aimingDirection) { }
	

}
