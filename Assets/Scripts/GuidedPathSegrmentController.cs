using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEditor;


public class GuidedPathSegrmentController : MonoBehaviour
{
	public GameObject _entryGate;
	public GameObject _exitGate;
	public PathCreator _path;
	PathCreator flattenedPath;

	private bool _checkForPathJoin = false;
	private string _cachedGateName;

	private PlayerController _player;
	private float _playerRadius;

	private void Start()
	{
		_player = GameObject.Find("CharacterFrameContainer").GetComponentInChildren<PlayerController>();
		_playerRadius = _player.gameObject.GetComponent<CapsuleCollider>().radius;
		flattenedPath = _path;
		flattenedPath.bezierPath.Space = PathSpace.xz;
		flattenedPath.TriggerPathUpdate();
	}

	public void BindPathToGates()
	{
		_path.bezierPath.SetPoint(0, _entryGate.transform.localPosition - _entryGate.transform.right * 1.5f);
		_path.bezierPath.SetPoint(1, _entryGate.transform.localPosition);
		_path.bezierPath.SetPoint(_path.bezierPath.NumPoints-1, _exitGate.transform.localPosition + _exitGate.transform.right * 1.5f);
		_path.bezierPath.SetPoint(_path.bezierPath.NumPoints-2, _exitGate.transform.localPosition);
	}

	private void Update()
	{
		if(_checkForPathJoin)
		{
			MovePlayerToPath(_cachedGateName);
		}
	}

	public void MovePlayerToPath(string gateName)
	{
		_cachedGateName = gateName;
		float startingPosition;
		bool canMoveToPath = false;
		if(gateName.Contains("Entry"))
		{
			startingPosition = 0;
			if (_player._facingDirection == PlayerController.Direction.Right)
			{
				canMoveToPath = true;
			}
			else
			{
				_checkForPathJoin = true;
			}
		}
		else
		{
			startingPosition = _path.path.length;
			if (_player._facingDirection == PlayerController.Direction.Left)
			{
				canMoveToPath = true;
			}
			else
			{
				_checkForPathJoin = true;
			}
		}
		
		if(canMoveToPath && !_player._followingPath)
		{
			_player.EnableFollowPath(flattenedPath, startingPosition);
		}
		
	}

	public void StopCheckingForPathJoin()
	{
		_checkForPathJoin = false;
	}
}
