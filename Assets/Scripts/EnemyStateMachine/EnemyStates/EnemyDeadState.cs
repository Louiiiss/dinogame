using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
	public override void Enter()
	{
		base.Enter();
		Enemy._animator.SetBool("Dead", true);
		Enemy.SetInvulnerable();
		Enemy.DisableGravity();
	}
}
