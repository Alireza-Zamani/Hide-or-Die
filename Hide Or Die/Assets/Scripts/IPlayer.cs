using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{

	void AddComponent(string component);


	bool HasTrapGetter();

	void HasTrapSetter(bool activity);

	string TeamGetter();

	void TeamSetter(string team);

	void AddAbility();

	void TakeDamage(float damageAmount);

	void Heal(float healAmount);

	void StuckPlayer(float timeRate);

}
