using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
	protected EnemyController Enemy;
	protected EnemyStateMachine StateMachine;

	public virtual void Init(EnemyController enemyController, EnemyStateMachine stateMachine)
	{
		Enemy = enemyController;
		StateMachine = stateMachine;
	}

	public virtual void Enter()
	{

	}

	public virtual void DoUpdate()
	{

	}

	public virtual void DoFixedUpdate()
	{

	}

	public virtual void Exit()
	{

	}
}
