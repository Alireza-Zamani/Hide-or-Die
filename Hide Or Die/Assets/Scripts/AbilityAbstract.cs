using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class AbilityAbstract : MonoBehaviourPunCallbacks
{
	public virtual void ExecuteAbility(Vector2 aimingDirection) { }

}
