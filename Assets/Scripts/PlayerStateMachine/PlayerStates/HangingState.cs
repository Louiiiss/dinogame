using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._playerAnimator.SetBool("Hanging", true);
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();

		Player.GetMovementInput_Aerial();

		if (Player._currentJumpingSpeed > -(Player._hangingThreshold * Player._hangTimeJumpSpeed))
		{
			Player.UpdateJump(0.33f);
		}
		else
		{
			StateMachine.ChangeState(StateMachine.StateName.Falling);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Debug.Log("Exiting");
		Player._playerAnimator.SetBool("Hanging", false);
	}

}
