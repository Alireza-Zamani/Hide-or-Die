using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{


	string TeamGetter();

	void TeamSetter(string team);

	void AddAbility();

	void TakeDamage(float damageAmount);

	void Heal(float healAmount);

}
