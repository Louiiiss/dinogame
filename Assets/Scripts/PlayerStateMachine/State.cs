using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
	protected PlayerController Player;
	protected StateMachine StateMachine;

	public virtual void Init(PlayerController playerMovementController, StateMachine stateMachine)
	{
		Player = playerMovementController;
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
