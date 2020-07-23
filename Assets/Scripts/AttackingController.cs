using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingController : MonoBehaviour
{
	private float _currentDamage;

	public void SetDamageValue(float value)
	{
		_currentDamage = value;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Dealing Damage");
		other.SendMessage("TakeDamage", _currentDamage);
	}
}
