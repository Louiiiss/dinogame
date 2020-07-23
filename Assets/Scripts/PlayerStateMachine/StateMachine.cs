using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	private Dictionary<StateName, State> _states;
	[System.Serializable]
	public struct StateInfo
	{
		public StateMachine.StateName name;
		public State state;
	}
	public StateInfo CurrentState;
	private PlayerController _playerController;

	public enum StateName
	{
		Standing,
		Jumping,
		Hanging,
		Falling,
		Landing,
		Eating,
		Attacking,
		Turning,
		CrouchingDown,
		Crouching,
		StandingUp,
		CrouchTurning,
		Resting,
		EndResting,
		None
	}

	public void Initialize(Dictionary<StateName, State> states, StateName startingState, PlayerController playerMovementController)
	{
		_states = states;
		_playerController = playerMovementController;
		CurrentState.name = startingState;
		CurrentState.state = _states[startingState];
		CurrentState.state.Enter();
	}

	public void ChangeState(StateName newState)
	{
		CurrentState.state.Exit();

		CurrentState.name = newState;
		CurrentState.state = _states[newState];
		CurrentState.state.Enter();
		Debug.Log("Transitioning to " + CurrentState.name);
	}

	public IEnumerator ChangeStateAfterDelay(StateName newState, float delay)
	{
		CurrentState.state.Exit();

		yield return new WaitForSeconds(delay);

		CurrentState.name = newState;
		CurrentState.state = _states[newState];
		CurrentState.state.Enter();
	}

	public void UpdateCurrentState()
	{
		CurrentState.state.DoUpdate();
	}

	public void FixedUpdateCurrentState()
	{
		CurrentState.state.DoFixedUpdate();
	}
}
