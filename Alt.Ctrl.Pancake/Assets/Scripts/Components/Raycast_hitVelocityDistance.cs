using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Raycast_hitVelocityDistance : Raycast_hit
{

	private Rigidbody rigid;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
	}

	private void UpdateDistance()
	{
		// work out the velocity of the next move
		Vector3 velocity = rigid.velocity;
		velocity += Physics.gravity * Time.deltaTime;

		// work out the max distance that are are going to move in in the next update
		Vector3 nextPosition = transform.position + velocity;

		distance = Vector3.Distance(transform.position, velocity);
		
	}

}
