using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Raycast_hitVelocityDistance : Raycast_hit
{

	private Rigidbody rigid;
	protected Vector3 NextPosition { get; private set; }

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
	}

	protected virtual void FixedUpdate()
	{
		UpdateDistance();
	}

	private void UpdateDistance()
	{
		// work out the velocity of the next move
		Vector3 velocity = rigid.velocity;				// vel per second

		velocity *= Time.deltaTime;						// total vel this update
		//velocity += Physics.gravity * Time.deltaTime;	// add this moves velocity


		// work out the max distance that are are going to move in in the next update
		NextPosition = transform.position + velocity;

		distance = Vector3.Distance(transform.position, velocity);
		print( "NP: "+NextPosition +" vel: "+rigid.velocity+" t: "+velocity );
	}

}
