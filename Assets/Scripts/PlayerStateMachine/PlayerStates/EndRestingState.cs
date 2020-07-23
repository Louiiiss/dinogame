using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRestingState : State
{
	public override void DoUpdate()
	{
		base.DoUpdate();

		if (Player._playerAnimator.GetBool("ReturnToIdle") == true)
		{
			Player._playerAnimator.SetBool("ReturnToIdle", false);
			StateMachine.ChangeState(StateMachine.StateName.Standing);
		}
	}
}
