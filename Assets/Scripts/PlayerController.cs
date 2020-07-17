using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[HideInInspector]
	public float _currentHealth;
	[HideInInspector]
	public float _currentSpeed;
	[HideInInspector]
	public float _new_speed;
	[HideInInspector]
	public float _currentJumpingSpeed;
	[HideInInspector]
	public Animator _playerAnimator;
	[HideInInspector]
	public Vector3 _currentPostion;
	[HideInInspector]
	public Vector3 _currentRotation;
	[HideInInspector]
	public LandingController _landingController;
	[HideInInspector]
	public EatingController _eatingController;
	[HideInInspector]
	public Rigidbody _rigidbody;
	[HideInInspector]
	private Direction _facingDirection;


	public float _maxHealth;
	public float _maxSpeed;
	public float _acceleration;
	public float _speedDecay;
	public float _jumpSpeed;
	public float _jumpDecay;
	public float _maxFallSpeed;
	public float _idleThreshold;
	public float _quickTurnSpeed;
	[Range(0.0f, 1.0f)]
	public float _quickTurnThreshold;
	[Range(0.0f, 1.0f)]
	public float _hangingThreshold;
	public float _aerialSpeed;
	public GameObject _landingCollider;
	public GameObject _eatingCollider;


	public delegate void UIUpdateAction();
	public event UIUpdateAction TriggerUIUpdate;


	public StateMachine.StateInfo[] _states;
	private StateMachine _stateMachine;

	private enum Direction
	{
		Left,
		Right
	}

	private void InitialiseStateMachine()
	{
		Dictionary<StateMachine.StateName, State> stateDictionary = new Dictionary<StateMachine.StateName, State>();
		_stateMachine = gameObject.AddComponent<StateMachine>();

		foreach (StateMachine.StateInfo info in _states)
		{
			if(info.state != null)
			{
				info.state.Init(this, _stateMachine);
				stateDictionary[info.name] = info.state;
			}
		}
		
		_stateMachine.Initialize(stateDictionary, StateMachine.StateName.Standing, this);
	}


	private void Start()
	{
		_facingDirection = Direction.Right;
		CorrectFacingDirection();
		InitialiseStateMachine();

		GetGenericInformation();
		_playerAnimator.SetFloat("Speed", _currentSpeed);

		_rigidbody = this.GetComponent<Rigidbody>();

		InitialisePlayerStats();
	}

	private void InitialisePlayerStats()
	{
		_currentSpeed = 0;
		_new_speed = 0;
		_currentHealth = _maxHealth;
		TriggerUIUpdate();
	}

	private void OnEnable()
	{
		_playerAnimator = this.GetComponent<Animator>();
		_landingController = _landingCollider.GetComponent<LandingController>();
		_eatingController = _eatingCollider.GetComponent<EatingController>();

		_landingController.TriggerLanding += TriggerLanding;
		_landingController.TriggerFalling += TriggerFalling;
	}

	private void OnDisable()
	{
		_landingController.TriggerLanding -= TriggerLanding;
		_landingController.TriggerFalling -= TriggerFalling;
	}

	private void Update()
	{
		GetGenericInformation();
		_stateMachine.UpdateCurrentState();
	}

	private void FixedUpdate()
	{
		_stateMachine.FixedUpdateCurrentState();
	}

	//// State Transitions
	
	public void CorrectFacingDirection()
	{
		Vector3 correctedDirection;
		if (_facingDirection == Direction.Left)
		{
			correctedDirection = new Vector3(_currentRotation.x, -90, _currentRotation.z);
}
		else
		{
			correctedDirection = new Vector3(_currentRotation.x, 90, _currentRotation.z);
		}

		this.transform.rotation = Quaternion.Euler(correctedDirection);

	}


	// Utilities
	public void SetSpeed(float speed)
	{
		_currentSpeed = speed;
		_playerAnimator.SetFloat("Speed", Mathf.Abs(_currentSpeed));
		_currentPostion = this.transform.position;
		Vector3 newPosition = _currentPostion + new Vector3(_currentSpeed * Time.fixedDeltaTime, 0f, 0f);
		//this.transform.position = newPosition;
		_rigidbody.MovePosition(newPosition);
	}

	public void UpdateJump(Vector3 newPosition)
	{
		_rigidbody.MovePosition(newPosition);
		_currentJumpingSpeed = Mathf.Clamp(_currentJumpingSpeed - (_jumpDecay * Time.fixedDeltaTime), -_maxFallSpeed, _jumpSpeed);
	}

	private void GetGenericInformation()
	{
		_currentPostion = this.transform.position;
		_currentRotation = this.transform.rotation.eulerAngles;
	}

	public void GetMovementInput_Standing(bool aerial=false,bool restrictTurning=false)
	{
		_new_speed = _currentSpeed;
		if (Input.GetKey(KeyCode.RightArrow))
		{
			float totalAcceleration = _acceleration;
			float quickturnspeed = 0;
			if (_currentSpeed < 0)
			{
				totalAcceleration += _speedDecay;
				if (_currentSpeed < -(_quickTurnThreshold * _maxSpeed))
				{
					quickturnspeed = _quickTurnSpeed;
				}
				else if(aerial)
				{
					quickturnspeed = _aerialSpeed;
				}
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;
			
			if (aerial)
			{
				speedUpperBound = _aerialSpeed;
				speedLowerBound = -_aerialSpeed;
				totalAcceleration = totalAcceleration * 2;
			}
			if (restrictTurning)
			{
				speedLowerBound = 0;
			}
			_new_speed = Mathf.Clamp((_currentSpeed + (totalAcceleration * Time.deltaTime)) + quickturnspeed, speedLowerBound, speedUpperBound);

		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			float totalAcceleration = _acceleration;
			float quickturnspeed = 0;
			if (_currentSpeed > 0)
			{
				totalAcceleration += _speedDecay;
				if (_currentSpeed > (_quickTurnThreshold * _maxSpeed))
				{
					quickturnspeed = _quickTurnSpeed;
				}
				else if (aerial)
				{
					quickturnspeed = _aerialSpeed;
				}
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;

			if (aerial)
			{
				speedUpperBound = _aerialSpeed;
				speedLowerBound = -_aerialSpeed;
				totalAcceleration = totalAcceleration * 2;
			}
			if (restrictTurning)
			{
				speedUpperBound = 0;
			}
			_new_speed = Mathf.Clamp((_currentSpeed - (totalAcceleration * Time.deltaTime)) - quickturnspeed, speedLowerBound, speedUpperBound);
		}
		else if ((aerial || restrictTurning) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
		{
			_new_speed = 0f;
		}
		else if (_currentSpeed != 0)
		{

			float relativeDecay = _speedDecay * Mathf.Sign(_currentSpeed);
			_new_speed = Mathf.Clamp((_currentSpeed - (relativeDecay * Time.deltaTime)), -_maxSpeed, _maxSpeed);
			if (_new_speed < _idleThreshold && _new_speed > -_idleThreshold)
			{
				_new_speed = 0f;
			}
		}

		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			_stateMachine.ChangeState(StateMachine.StateName.CrouchingDown);
		}

		if ((_facingDirection == Direction.Right && _new_speed < 0) || (_facingDirection == Direction.Left && _new_speed > 0))
		{
			if (_new_speed != 0 && _stateMachine.CurrentState.name == StateMachine.StateName.Standing)
			{
				_stateMachine.ChangeState(StateMachine.StateName.Turning);
			}
		}

		SetSpeed(_new_speed);
	}

	public void GetMovementInput_Crouching()
	{
		_new_speed = _currentSpeed;
		if (Input.GetKey(KeyCode.RightArrow))
		{
			float totalAcceleration = _acceleration;
			if (_currentSpeed < 0)
			{
				totalAcceleration += _speedDecay;
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;

			_new_speed = Mathf.Clamp((_currentSpeed + (totalAcceleration * Time.deltaTime)), speedLowerBound, speedUpperBound);

		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			float totalAcceleration = _acceleration;
			if (_currentSpeed > 0)
			{
				totalAcceleration += _speedDecay;

			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;
			_new_speed = Mathf.Clamp((_currentSpeed - (totalAcceleration * Time.deltaTime)), speedLowerBound, speedUpperBound);
		}
		else if (_currentSpeed != 0)
		{

			float relativeDecay = _speedDecay * Mathf.Sign(_currentSpeed);
			_new_speed = Mathf.Clamp((_currentSpeed - (relativeDecay * Time.deltaTime)), -_maxSpeed, _maxSpeed);
			if (_new_speed < _idleThreshold && _new_speed > -_idleThreshold)
			{
				_new_speed = 0f;
			}
		}

		if (!Input.GetKey(KeyCode.DownArrow))
		{
			_stateMachine.ChangeState(StateMachine.StateName.StandingUp);
		}

		if ((_facingDirection == Direction.Right && _new_speed < 0) || (_facingDirection == Direction.Left && _new_speed > 0))
		{
			if (_new_speed != 0 && _stateMachine.CurrentState.name == StateMachine.StateName.Standing)
			{
				_stateMachine.ChangeState(StateMachine.StateName.Turning);
			}
		}

		SetSpeed(_new_speed);
	}


	public void GetActionInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			StartCoroutine("Jump");
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			Eat();
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Attack();
		}

		if(Input.GetKeyDown(KeyCode.J))
		{
			TakeDamage(10f);
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(-10f);
		}
	}

	//// Actions
	private void Jump()
	{
		_playerAnimator.SetBool("Jumping", true);
		IEnumerator coroutine = _stateMachine.ChangeStateAfterDelay(StateMachine.StateName.Jumping, 0.2f);
		StartCoroutine(coroutine);
	}

	private void Eat()
	{
		// Do eating logic
		_stateMachine.ChangeState(StateMachine.StateName.Eating);
		if(_eatingController._foodInRange)
		{
			TakeDamage(-10f);
		}
	}

	private void Attack()
	{
		// Do eating logic
		_stateMachine.ChangeState(StateMachine.StateName.Attacking);
	}

	public void Turn()
	{
		Debug.Log("Turning");
		if (_facingDirection == Direction.Right)
		{
			_facingDirection = Direction.Left;
			_playerAnimator.SetBool("TurningRight", false);
			_playerAnimator.SetBool("TurningLeft", true);
		}
		else if (_facingDirection == Direction.Left)
		{
			_facingDirection = Direction.Right;
			_playerAnimator.SetBool("TurningLeft", false);
			_playerAnimator.SetBool("TurningRight", true);
		}
	}

	private void TriggerLanding()
	{
		Debug.Log("Landing");
		if (_stateMachine.CurrentState.name == StateMachine.StateName.Standing || _stateMachine.CurrentState.name == StateMachine.StateName.Hanging || _stateMachine.CurrentState.name == StateMachine.StateName.Falling)
		{
			_stateMachine.ChangeState(StateMachine.StateName.Landing);
		}
	}

	private void TriggerFalling()
	{
		Debug.Log("Falling");
		if (_stateMachine.CurrentState.name == StateMachine.StateName.Standing || _stateMachine.CurrentState.name == StateMachine.StateName.Hanging || _stateMachine.CurrentState.name == StateMachine.StateName.Landing)
		{
			_stateMachine.ChangeState(StateMachine.StateName.Falling);
		}
	}

	private void TakeDamage(float amount)
	{
		_currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
		Debug.Log(_currentHealth);
		TriggerUIUpdate();
	}

	//private void OnColl
}
