using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._rigidbody.useGravity = false;
		Player._currentJumpingSpeed = Player._jumpSpeed;
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();

		Player.GetMovementInput_Standing(aerial:true);

		if (Player._currentJumpingSpeed < (Player._hangingThreshold * Player._jumpSpeed))
		{
			StateMachine.ChangeState(StateMachine.StateName.Hanging);
		}
		else
		{
			Vector3 newPosition = Player._currentPostion + new Vector3(Player._currentSpeed * Time.deltaTime, (Player._currentJumpingSpeed * Time.deltaTime), 0f);
			Player.UpdateJump(newPosition);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("Jumping", false);
	}

}
