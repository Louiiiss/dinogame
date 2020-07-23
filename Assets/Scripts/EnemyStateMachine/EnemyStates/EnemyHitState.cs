using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyState
{
	public override void Enter()
	{
		base.Enter();
		Enemy._animator.SetBool("Hit", true);
		Enemy.SetInvulnerable();
	}

	public override void DoUpdate()
	{
		base.DoUpdate();

		if (Enemy._animator.GetBool("ReturnToIdle") == true)
		{
			Enemy._animator.SetBool("ReturnToIdle", false);
			StateMachine.ChangeState(EnemyStateMachine.EnemyStateName.Idle);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Enemy._animator.SetBool("Hit", false);
	}

}
