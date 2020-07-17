using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingController : MonoBehaviour
{

	public bool _foodInRange = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Food")
		{
			_foodInRange = true;
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
