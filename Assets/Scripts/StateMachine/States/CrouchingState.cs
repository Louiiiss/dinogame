using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchingState : State
{
	public override void Enter()
	{
		base.Enter();
		//Player.CorrectFacingDirection();
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Crouching();
	}
	public override void DoUpdate()
	{
		base.DoUpdate();
		//Player.GetActionInput();
	}
}
