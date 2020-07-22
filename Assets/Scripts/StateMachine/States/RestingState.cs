using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingState : State
{
	public override void Enter()
	{
		base.Enter();
		Player._playerAnimator.SetBool("Resting", true);
		Player.RestoreToFullHealth();
	}

	public override void DoUpdate()
	{
		base.DoUpdate();
		if(Input.GetKeyDown(KeyCode.E))
		{
			Player.EndResting();
		}
	}

	public override void Exit()
	{
		base.Exit();
		Player._playerAnimator.SetBool("Resting", false);
	}
}
