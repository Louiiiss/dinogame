using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingController : MonoBehaviour
{

	public delegate void TriggerAction();
	public event TriggerAction TriggerLanding;
	public event TriggerAction TriggerFalling;

	private void OnTriggerEnter(Collider other)
	{
		TriggerLanding();
	}

	private void OnTriggerExit(Collider other)
	{
		TriggerFalling();
	}
}
