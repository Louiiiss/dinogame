using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public Vector3 offset;
	public float testoffset;

	// Update is called once per frame
	void FixedUpdate()
	{
		if (target)
		{
			Vector3 delta = target.position + (target.forward*offset.x) + (target.up * offset.y) + (target.right*offset.z);
			transform.position = Vector3.Lerp(transform.position, delta, dampTime);
			Vector3 rotation = target.rotation.eulerAngles + new Vector3(0f, -90f, 0f);
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation),dampTime);
		}

	}
}
