using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : State
{
	private float baseMagnitude = 4f;
	private float elapsedTime = 0f;

	public override void Enter()
	{
		base.Enter();
		Player._rigidbody.useGravity = false;
		Player._playerAnimator.SetBool("Falling", true);
		Player.SetFallMagnitude(baseMagnitude);
		elapsedTime = 0f;
	}

	public override void DoUpdate()
	{
		base.DoUpdate();
		elapsedTime += Time.deltaTime;
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Aerial();
		Player.UpdateJump();
	}

	public override void Exit()
	{
		float magnitude = Mathf.Clamp(elapsedTime * 12f, baseMagnitude, 10f);
		Player.SetFallMagnitude(magnitude);
		Player._playerAnimator.SetBool("Falling", false);
		Player.StopFallingCheck();
	}
}
