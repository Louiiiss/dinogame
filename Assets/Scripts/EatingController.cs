using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingController : MonoBehaviour
{
	public delegate void TriggerAction(float value);
	public event TriggerAction TriggerHealing;
	private bool _foodInRange = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Food")
		{
			_foodInRange = true;
			float _healValue = other.GetComponentInParent<EnemyController>().ConsumeFoodCharge();
			TriggerHealing(_healValue);
			Debug.Log("Food Value " + _healValue.ToString());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Food")
		{
			_foodInRange = false;
		}
	}
}
