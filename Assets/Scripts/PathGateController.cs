using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGateController : MonoBehaviour
{
	private GuidedPathSegrmentController _guidedPathSegrmentController;

	private void Start()
	{
		_guidedPathSegrmentController = this.GetComponentInParent<GuidedPathSegrmentController>();
	}


	private void OnTriggerEnter(Collider other)
	{
		_guidedPathSegrmentController.MovePlayerToPath(this.gameObject.name);
	}

	private void OnTriggerExit(Collider other)
	{
		_guidedPathSegrmentController.StopCheckingForPathJoin();
	}
}
