using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class AbilityAbstract : MonoBehaviourPunCallbacks
{

	public virtual void AbilityIsStarting(GameObject aimingPref) { }

	public virtual void ExecuteAbility() { }



}
