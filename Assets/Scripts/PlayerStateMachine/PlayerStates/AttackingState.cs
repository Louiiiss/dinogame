using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._playerAnimator.SetBool("Attacking", true);
		Player.SetSpeed(0f);
		Player.EnableContainerRootMotion();
		//Player._rigidbody.useGravity = false;
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
		Player._playerAnimator.SetBool("Attacking", false);
		Player.DisableContainerRootMotion();
		Player._rigidbody.useGravity = true;
		Player.StopFallingCheck();
		Player.StopLandingCheck();
	}
}
