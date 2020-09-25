using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingState : State
{
	public override void Enter()
	{
		Player._rigidbody.useGravity = true;
		Player._playerAnimator.SetBool("Landing", true);
		Player.StopFallingCheck();
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

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Standing(restrictTurning: true);
		Player.MatchGroundNormal();
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("Landing", false);
		Player.StopLandingCheck();
	}
}
