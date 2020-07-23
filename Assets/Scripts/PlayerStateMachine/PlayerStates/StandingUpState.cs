using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingUpState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._playerAnimator.SetBool("StandingUp", true);
	}

	public override void DoUpdate()
	{
		base.DoUpdate();

		if (Player._playerAnimator.GetBool("ReturnToIdle") == true)
		{
			Player._playerAnimator.SetBool("ReturnToIdle", false);
			StateMachine.ChangeState(StateMachine.StateName.Standing);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("StandingUp", false);
	}
}
