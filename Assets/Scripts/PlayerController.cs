using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerController : MonoBehaviour
{
	[HideInInspector]
	public bool _followingPath;
	public PathCreator _currentPath;
	float _pathProgress = 0;

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
	public Vector3 _currentPosition;
	[HideInInspector]
	public Vector3 _currentRotation;
	[HideInInspector]
	public LandingController _landingController;
	[HideInInspector]
	public EatingController _eatingController;
	[HideInInspector]
	public AttackingController _attackingController;
	[HideInInspector]
	public Rigidbody _rigidbody;
	[HideInInspector]
	public Direction _facingDirection;


	public float _maxHealth;
	public float _maxSpeed;
	public float _maxCrouchSpeed;
	public float _acceleration;
	public float _speedDecay;
	public float _hangTimeJumpSpeed;
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
	public GameObject _attackingCollider;
	public float _baseAttackDamage;

	private bool _checkForLanding;
	private bool _checkForFalling;
	private Quaternion _groundAdjustedHeading;

	public delegate void UIUpdateAction();
	public event UIUpdateAction TriggerUIUpdate;

	private bool _updateContainerRootMotion;
	private Transform CharacterFrameContainer;

	public StateMachine.StateInfo[] _states;
	private StateMachine _stateMachine;

	public enum Direction
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
			if (info.state != null)
			{
				info.state.Init(this, _stateMachine);
				stateDictionary[info.name] = info.state;
			}
		}

		_stateMachine.Initialize(stateDictionary, StateMachine.StateName.Standing, this);
	}


	private void Start()
	{
		CharacterFrameContainer = this.transform.parent;
		_facingDirection = Direction.Right;
		_checkForLanding = false;
		_checkForFalling = true;
		CorrectFacingDirection();
		InitialiseStateMachine();

		GetGenericInformation();
		_playerAnimator.SetFloat("Speed", _currentSpeed);

		_rigidbody = CharacterFrameContainer.GetComponent<Rigidbody>();

		InitialisePlayerStats();
		_attackingController.SetDamageValue(_baseAttackDamage);

		_followingPath = false;
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
		_attackingController = _attackingCollider.GetComponent<AttackingController>();

		_landingController.TriggerLanding += TriggerLanding;
		_landingController.TriggerFalling += TriggerFalling;
		_landingController.CancelTriggerLanding += StopLandingCheck;
		_landingController.CancelTriggerFalling += StopFallingCheck;
		_eatingController.TriggerHealing += Heal;
	}

	private void OnDisable()
	{
		_landingController.TriggerLanding -= TriggerLanding;
		_landingController.TriggerFalling -= TriggerFalling;
		_landingController.CancelTriggerLanding -= StopLandingCheck;
		_landingController.CancelTriggerFalling -= StopFallingCheck;
		_eatingController.TriggerHealing -= Heal;
	}

	private void Update()
	{
		GetGenericInformation();
		_stateMachine.UpdateCurrentState();

		if (_checkForLanding)
		{
			TryPerformLanding();
		}
		if (_checkForFalling)
		{
			TryPerformFalling();
		}
	}

	private void FixedUpdate()
	{
		_stateMachine.FixedUpdateCurrentState();
	}

	//// State Transitions

	public void CorrectFacingDirection()
	{
		if (_facingDirection == Direction.Left)
		{
			this.transform.rotation = GetHeadingFromGroundNormal(-CharacterFrameContainer.right);
		}
		else
		{
			this.transform.rotation = GetHeadingFromGroundNormal(CharacterFrameContainer.right);
		}
		
	}

	private float _cachedPathProgress;

	// Utilities
	public void SetSpeed(float speed)
	{
		_currentSpeed = speed;
		_playerAnimator.SetFloat("Speed", Mathf.Abs(_currentSpeed));
		_currentPosition = CharacterFrameContainer.position;
		Vector3 newPosition;
		if (_followingPath)
		{
			float distance_to_move = _currentSpeed * Time.fixedDeltaTime;
			_cachedPathProgress = _pathProgress;
			_pathProgress += distance_to_move;

			if (_pathProgress >= _currentPath.path.length || _pathProgress < 0)
			{
				_followingPath = false;
				newPosition = _currentPosition + (CharacterFrameContainer.forward * _currentSpeed * Time.fixedDeltaTime);
				_rigidbody.MovePosition(newPosition);
			}
			else
			{
				newPosition = new Vector3(_currentPath.path.GetPointAtDistance(_pathProgress).x, _rigidbody.position.y, _currentPath.path.GetPointAtDistance(_pathProgress).z);
				_rigidbody.MovePosition(newPosition);
				Vector3 newRotation = new Vector3(_rigidbody.rotation.x, _currentPath.path.GetRotationAtDistance(_pathProgress).eulerAngles.y, _rigidbody.rotation.z);
				CharacterFrameContainer.rotation = Quaternion.Euler(newRotation);
			}
			Vector3 normalized_height_position = new Vector3(_currentPosition.x, 0f, _currentPosition.z);
			float path_progress2 = _currentPath.path.GetClosestDistanceAlongPath(normalized_height_position) + distance_to_move;
			if (Mathf.Abs(_pathProgress - path_progress2) > 0.15f)
			{
				_pathProgress = _cachedPathProgress;
			}
		}
		else
		{
			newPosition = _currentPosition + (CharacterFrameContainer.forward * _currentSpeed * Time.fixedDeltaTime);
			_rigidbody.MovePosition(newPosition);
		}

		
		_currentPosition = newPosition;
	}

	public void UpdateJump(float jumpSpeedModifier = 1.0f)
	{
		Vector3 newJumpPosition = _currentPosition + new Vector3(0f, _currentJumpingSpeed * jumpSpeedModifier * Time.fixedDeltaTime,0f);
		_rigidbody.MovePosition(newJumpPosition);
		_currentJumpingSpeed = Mathf.Clamp(_currentJumpingSpeed - (_jumpDecay * Time.fixedDeltaTime), -_maxFallSpeed, _hangTimeJumpSpeed);
	}

	private void GetGenericInformation()
	{
		_currentPosition = this.transform.position;
		_currentRotation = this.transform.rotation.eulerAngles;
	}

	public void GetMovementInput_Standing(bool restrictTurning = false)
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
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;

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
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;

			if (restrictTurning)
			{
				speedUpperBound = 0;
			}
			_new_speed = Mathf.Clamp((_currentSpeed - (totalAcceleration * Time.deltaTime)) - quickturnspeed, speedLowerBound, speedUpperBound);
		}
		else if ((restrictTurning) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
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

		if (Input.GetKey(KeyCode.DownArrow))
		{
			_stateMachine.ChangeState(StateMachine.StateName.CrouchingDown);
		}

		if (!restrictTurning && ((_facingDirection == Direction.Right && _new_speed < 0) || (_facingDirection == Direction.Left && _new_speed > 0)))
		{
			if (_new_speed != 0 && _stateMachine.CurrentState.name == StateMachine.StateName.Standing)
			{
				_stateMachine.ChangeState(StateMachine.StateName.Turning);
			}
		}

		SetSpeed(_new_speed);
	}

	public void GetMovementInput_Aerial(bool restrictTurning = false)
	{
		_new_speed = _currentSpeed;
		if (Input.GetKey(KeyCode.RightArrow))
		{
			float totalAcceleration = _acceleration;
			float quickturnspeed = _aerialSpeed;
			if (_currentSpeed < 0)
			{
				quickturnspeed = _aerialSpeed;
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;
			speedUpperBound = _aerialSpeed;
			speedLowerBound = -_aerialSpeed;
			totalAcceleration = totalAcceleration * 2;

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
				quickturnspeed = _aerialSpeed;
			}

			float speedUpperBound = _maxSpeed;
			float speedLowerBound = -_maxSpeed;

			speedUpperBound = _aerialSpeed;
			speedLowerBound = -_aerialSpeed;
			totalAcceleration = totalAcceleration * 2;
			if (restrictTurning)
			{
				speedUpperBound = 0;
			}
			_new_speed = Mathf.Clamp((_currentSpeed - (totalAcceleration * Time.deltaTime)) - quickturnspeed, speedLowerBound, speedUpperBound);
		}
		else if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
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

		if (!restrictTurning && ((_facingDirection == Direction.Right && _new_speed < 0) || (_facingDirection == Direction.Left && _new_speed > 0)))
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

			float speedUpperBound = _maxCrouchSpeed;
			float speedLowerBound = -_maxCrouchSpeed;

			_new_speed = Mathf.Clamp((_currentSpeed + (totalAcceleration * Time.deltaTime)), speedLowerBound, speedUpperBound);

		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			float totalAcceleration = _acceleration;
			if (_currentSpeed > 0)
			{
				totalAcceleration += _speedDecay;

			}

			float speedUpperBound = _maxCrouchSpeed;
			float speedLowerBound = -_maxCrouchSpeed;
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
			if (_new_speed != 0 && _stateMachine.CurrentState.name == StateMachine.StateName.Crouching)
			{
				_stateMachine.ChangeState(StateMachine.StateName.CrouchTurning);
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

		if (Input.GetKeyDown(KeyCode.C))
		{
			Eat();
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Attack();
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			TakeDamage(10f);
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(-10f);
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			Rest();
		}
	}

	//// Actions
	private void Jump()
	{
		_playerAnimator.SetBool("Jumping", true);
		//IEnumerator coroutine = _stateMachine.ChangeStateAfterDelay(StateMachine.StateName.Jumping, 0.2f);
		//StartCoroutine(coroutine);
		_stateMachine.ChangeState(StateMachine.StateName.Jumping);
	}

	private void Eat()
	{
		// Do eating logic
		_stateMachine.ChangeState(StateMachine.StateName.Eating);
	}

	public void Attack()
	{
		// Do eating logic
		_stateMachine.ChangeState(StateMachine.StateName.Attacking);
	}

	private void Rest()
	{
		_stateMachine.ChangeState(StateMachine.StateName.Resting);
	}

	public void EndResting()
	{
		_stateMachine.ChangeState(StateMachine.StateName.EndResting);
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
		_checkForLanding = true;
	}

	private void TryPerformLanding()
	{
		AnimatorStateInfo stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("Hanging") || stateInfo.IsName("Falling"))
		{
			_stateMachine.ChangeState(StateMachine.StateName.Landing);
		}
	}

	public void StopLandingCheck()
	{
		_checkForLanding = false;
	}

	private void TriggerFalling()
	{
		Debug.Log("Falling");
		_checkForFalling = true;
	}

	private void TryPerformFalling()
	{
		Debug.Log("Trying to fall");
		AnimatorStateInfo stateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("Standing Blend Tree") || stateInfo.IsName("Crouching Blend Tree"))
		{
			_stateMachine.ChangeState(StateMachine.StateName.Falling);
		}
	}

	public void StopFallingCheck()
	{
		Debug.Log("stopping falling check");
		_checkForFalling = false;
	}

	public void TakeDamage(float amount)
	{
		_currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
		Debug.Log(_currentHealth);
		TriggerUIUpdate();
	}

	public void Heal(float amount)
	{
		_currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
		Debug.Log(_currentHealth);
		TriggerUIUpdate();
	}

	public void RestoreToFullHealth()
	{
		_currentHealth = _maxHealth;
		TriggerUIUpdate();
	}

	private void OnAnimatorMove()
	{
		if(_updateContainerRootMotion)
		{
			if(_followingPath)
			{
				Vector3 closestPredictedPoint = _currentPath.path.GetClosestPointOnPath(CharacterFrameContainer.position);
				Vector3 adjustedPrediction = new Vector3(closestPredictedPoint.x, CharacterFrameContainer.position.y, closestPredictedPoint.z);
				Vector3 current_plus_delta = CharacterFrameContainer.position + _playerAnimator.deltaPosition;
				Vector3 predicted_current_plus_delta_on_path = _currentPath.path.GetClosestPointOnPath(current_plus_delta);
				Vector3 adjusted_predicted_current_plus_delta_on_path = new Vector3(predicted_current_plus_delta_on_path.x, current_plus_delta.y, predicted_current_plus_delta_on_path.z);
				Vector3 pathDelta = adjusted_predicted_current_plus_delta_on_path - adjustedPrediction;
				Vector3 calculatedPathPosition = CharacterFrameContainer.position + _playerAnimator.deltaPosition;
				_pathProgress = _currentPath.path.GetClosestDistanceAlongPath(calculatedPathPosition);
				CharacterFrameContainer.position += pathDelta;
				Vector3 newRotation = new Vector3(_rigidbody.rotation.x, _currentPath.path.GetRotationAtDistance(_pathProgress).eulerAngles.y, _rigidbody.rotation.z);
				CharacterFrameContainer.rotation = Quaternion.Euler(newRotation);
			}
			else
			{
				CharacterFrameContainer.position += _playerAnimator.deltaPosition;
			}
			
		}
		else
		{
			_playerAnimator.ApplyBuiltinRootMotion();
		}
	}

	public void EnableContainerRootMotion()
	{
		_updateContainerRootMotion = true;
	}

	public void DisableContainerRootMotion()
	{
		_updateContainerRootMotion = false;
	}

	public void ClampCurrentSpeedToNewMax(float max)
	{
		float clampedSpeed = Mathf.Clamp(_currentSpeed, -max, max);
		SetSpeed(clampedSpeed);
	}

	public void EnableFollowPath(PathCreator path, float startingPosition)
	{
		Debug.Log("Starting path at " + startingPosition.ToString());
		_followingPath = true;
		_currentPath = path;
		//_rigidbody.MovePosition(new Vector3(_currentPath.path.GetPointAtDistance(startingPosition).x,_currentPostion.y, _currentPath.path.GetPointAtDistance(startingPosition).z));
		_pathProgress = startingPosition;
	}

	public void DisableFollowPath()
	{
		_followingPath = false;
		_currentPath = null;
	}

	public void MatchGroundNormal()
	{	
		this.transform.rotation = Quaternion.Lerp(this.transform.rotation, GetHeadingFromGroundNormal(this.transform.right), 0.2f + Time.deltaTime);
	}

	public Quaternion GetHeadingFromGroundNormal(Vector3 fwd)
	{
		RaycastHit hit;
		if (Physics.Raycast(CharacterFrameContainer.transform.position + new Vector3(0, 0.2f, 0), Vector3.down, out hit, 10))
		{
			Debug.DrawRay(hit.point, hit.normal * 2f, Color.magenta);
			Vector3 heading = Vector3.Cross(fwd, hit.normal);
			Debug.DrawRay(hit.point, heading, Color.red);
			_groundAdjustedHeading = Quaternion.LookRotation(heading, hit.normal);
		}
		return _groundAdjustedHeading;
	}
}
