using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player.CorrectFacingDirection();
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Standing();
	}
	public override void DoUpdate()
	{
		base.DoUpdate();
		Player.GetActionInput();
	}
}
