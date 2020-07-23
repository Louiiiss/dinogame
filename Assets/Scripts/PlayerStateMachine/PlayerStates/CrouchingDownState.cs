using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchingDownState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._playerAnimator.SetBool("CrouchingDown", true);
	}

	public override void DoUpdate()
	{
		base.DoUpdate();

		if (Player._playerAnimator.GetBool("ReturnToIdle") == true)
		{
			Player._playerAnimator.SetBool("ReturnToIdle", false);
			StateMachine.ChangeState(StateMachine.StateName.Crouching);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("CrouchingDown", false);
	}
}
