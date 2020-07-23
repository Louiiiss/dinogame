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

		if (Player._currentJumpingSpeed > -(Player._hangingThreshold * Player._jumpSpeed))
		{
			Vector3 newPosition = Player._currentPostion + new Vector3(Player._currentSpeed * Time.deltaTime, ((Player._currentJumpingSpeed / 3) * Time.deltaTime), 0f);
			Player.UpdateJump(newPosition);
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
