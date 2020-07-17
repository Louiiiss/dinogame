using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToIdle : StateMachineBehaviour
{
	public bool _ignoreFirstTime = true;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if(!_ignoreFirstTime)
		{
			animator.SetBool("ReturnToIdle", true);
			animator.SetBool("TurningRight", false);
			animator.SetBool("TurningLeft", false);
		}
		else
		{
			_ignoreFirstTime = false;
		}
	}
}
