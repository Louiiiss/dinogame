using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
	private Dictionary<EnemyStateName, EnemyState> _states;
	[System.Serializable]
	public struct EnemyStateInfo
	{
		public EnemyStateName name;
		public EnemyState state;
	}
	public EnemyStateInfo CurrentState;
	private EnemyController _enemyController;

	public enum EnemyStateName
	{
		Idle,
		Hit,
		Dead,
		None
	}

	public void Initialize(Dictionary<EnemyStateName, EnemyState> states, EnemyStateName startingState, EnemyController enemyController)
	{
		_states = states;
		_enemyController = enemyController;
		CurrentState.name = startingState;
		CurrentState.state = _states[startingState];
		CurrentState.state.Enter();
	}

	public void ChangeState(EnemyStateName newState)
	{
		CurrentState.state.Exit();

		CurrentState.name = newState;
		CurrentState.state = _states[newState];
		CurrentState.state.Enter();
		Debug.Log("Transitioning to " + CurrentState.name);
	}

	public IEnumerator ChangeStateAfterDelay(EnemyStateName newState, float delay)
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
