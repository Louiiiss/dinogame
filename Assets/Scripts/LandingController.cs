using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{

	public delegate void TriggerAction();
	public event TriggerAction TriggerLanding;
	public event TriggerAction CancelTriggerLanding;
	public event TriggerAction CancelTriggerFalling;
	public event TriggerAction TriggerFalling;

	private int _overlappingObjects = 0;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer != LayerMask.NameToLayer("Sensors"))
		{
			Debug.Log("Collision!");
			_overlappingObjects++;
			TriggerLanding();
			CancelTriggerFalling();
		}
	}

	private void Update()
	{
		Debug.Log(_overlappingObjects);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Sensors"))
		{

			_overlappingObjects--;
			
			if(_overlappingObjects <= 0)
			{
				Debug.Log("Leaving");
				TriggerFalling();
				CancelTriggerLanding();
			}
		}
	}
}
