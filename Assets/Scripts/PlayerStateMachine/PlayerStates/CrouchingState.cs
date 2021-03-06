﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player.CorrectFacingDirection();
		Player.ClampCurrentSpeedToNewMax(Player._maxCrouchSpeed);
	}

	public override void DoFixedUpdate()
	{
		base.DoFixedUpdate();
		Player.GetMovementInput_Crouching();
		Player.MatchGroundNormal();
	}
	public override void DoUpdate()
	{
		base.DoUpdate();
		if (Input.GetKeyDown(KeyCode.Z))
		{
			Player.Attack();
		}
		//Player.GetActionInput();
	}
}
