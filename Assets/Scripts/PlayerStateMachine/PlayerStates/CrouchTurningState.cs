﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchTurningState : State
{
	public override void Enter()
	{
		base.Enter();
		Player.Turn();
	}

	public override void DoUpdate()
	{
		base.DoUpdate();

		if (Player._playerAnimator.GetBool("ReturnToIdle") == true)
		{
			Player._playerAnimator.SetBool("ReturnToIdle", false);
			StateMachine.ChangeState(StateMachine.StateName.Crouching);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("TurningLeft", false);
		Player._playerAnimator.SetBool("TurningRight", false);
	}
}
