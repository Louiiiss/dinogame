using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : State
{
	public override void Enter()
	{
		base.Enter();

		Player._rigidbody.useGravity = false;
		Player._playerAnimator.SetBool("Falling", true);
		
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Standing(aerial: true);

		Vector3 newPosition = Player._currentPostion + new Vector3(Player._currentSpeed * Time.fixedDeltaTime, (Player._currentJumpingSpeed * Time.fixedDeltaTime), 0f);
		Player.UpdateJump(newPosition);
	}

	public override void Exit()
	{
		Player._playerAnimator.SetBool("Falling", false);
	}
}
