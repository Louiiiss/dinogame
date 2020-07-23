using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float _maxHealth;
	private float _currentHealth;
	public int _maxFoodCharges;
	public float _foodHealValue;
	[HideInInspector]
	public Animator _animator;
	private bool _vulnerable;
	private int _currentFoodCharges;


	public EnemyStateMachine.EnemyStateInfo[] _states;
	private EnemyStateMachine _stateMachine;

	private void InitialiseStateMachine()
	{
		Dictionary<EnemyStateMachine.EnemyStateName, EnemyState> stateDictionary = new Dictionary<EnemyStateMachine.EnemyStateName, EnemyState>();
		_stateMachine = gameObject.AddComponent<EnemyStateMachine>();

		foreach (EnemyStateMachine.EnemyStateInfo info in _states)
		{
			if (info.state != null)
			{
				info.state.Init(this, _stateMachine);
				stateDictionary[info.name] = info.state;
			}
		}

		_stateMachine.Initialize(stateDictionary, EnemyStateMachine.EnemyStateName.Idle, this);
	}

	private void OnEnable()
	{
		_currentHealth = _maxHealth;
		_currentFoodCharges = _maxFoodCharges;
		_animator = this.GetComponent<Animator>();
	}

	private void Start()
	{
		InitialiseStateMachine();
	}

	public void Update()
	{
		_stateMachine.UpdateCurrentState();
	}


	public void TakeDamage(float damage)
	{
		if(_vulnerable)
		{
			_currentHealth -= Mathf.Clamp(damage, 0f, _maxHealth);
			if (_currentHealth > 0)
			{
				_stateMachine.ChangeState(EnemyStateMachine.EnemyStateName.Hit);
			}
			else
			{
				_stateMachine.ChangeState(EnemyStateMachine.EnemyStateName.Dead);
			}
		}
	}

	public float ConsumeFoodCharge()
	{
		float healthGained = 0f;
		if(_currentFoodCharges > 0)
		{
			healthGained = _foodHealValue;
		}
		_currentFoodCharges = Mathf.Clamp(_currentFoodCharges-1,0,_maxFoodCharges);
		return healthGained;
	}

	public void SetVulnerable()
	{
		_vulnerable = true;
	}

	public void SetInvulnerable()
	{
		_vulnerable = false;
	}

	public  void DisableGravity()
	{
		this.GetComponentInParent<Rigidbody>().useGravity = false;
	}
}
