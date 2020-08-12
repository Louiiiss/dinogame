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
		Player.GetMovementInput_Aerial();
		Player.UpdateJump();
	}

	public override void Exit()
	{
		Player._playerAnimator.SetBool("Falling", false);
		Player.StopFallingCheck();
	}
}
