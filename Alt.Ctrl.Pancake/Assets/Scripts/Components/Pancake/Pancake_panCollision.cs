using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Pancake_velocity ) )]
public class Pancake_panCollision : Raycast_hit, IPanCollider
{
	Pancake_velocity pancake_velocity;
	private Transform panColliderObj;
	/*private*/ public Vector3 positionInPan;          // the local position of the pancake in the pan

	[Header( "Collider things" )]
	[Range(0f, 2f)]
	[SerializeField] private float friction = 0.1f;

	[SerializeField] private float upforceThresshold = 1f;
	private float transformUpforceDistance = 0;
	private Vector3 transformedVelocity = Vector3.zero;

	void Awake()
	{
		pancake_velocity = GetComponent<Pancake_velocity>();
	}

	private void FixedUpdate()
	{

		// 
		if(transformedVelocity != Vector3.zero)
		{
			pancake_velocity.SetVelocity( transformedVelocity );//*/
			transformedVelocity = Vector3.zero;
		}

		// if there current no panColliderObj we need to be searching for it with the ray cast, if we are falling.
		// else if there is panColliderObj we need to move with pan.
		// also if we are in the pan we need to apply the velocity.

		if ( panColliderObj != null )                       // in pan
			a();
		else if ( pancake_velocity.Velocity.y <= 0f )       // falling
			b();
		else
			c();                                            // not in pan and not falling, what ever that is

	}

	//TODO: give me a real name (HELP!, its hot in this pan)
	private void a()
	{
		transform.eulerAngles = panColliderObj.eulerAngles;
		positionInPan += pancake_velocity.GetTravleDistance( Time.deltaTime );
		transform.position = panColliderObj.TransformPoint( positionInPan );

		// TODO: Take off... 
		// hmmm... i dont no where to do this.
		pancake_velocity.AddFriction(friction);

	}

	//TODO: can i also get a real name. (awwwwwwwwwwww, im falling...)
	private void b()
	{
		// Apply the physic step now so we can work out where we will be at the end of this update.
		pancake_velocity.PhysicsStep( Time.deltaTime );

		// find the max distance that we are going to move during the update.
		Vector3 nextPosition = transform.position + pancake_velocity.GetTravleDistance( Time.deltaTime );
		distance = Vector3.Distance( transform.position, /*pancake_velocity.Velocity );/*/ nextPosition );//*/	// << comment switch :D

		bool collision = false;

		// find if we collide with any objects
		if ( CastRay() )
		{
			if ( nextPosition.y <= rayHit.point.y )  // collided
			{
				nextPosition.y = rayHit.point.y;
				pancake_velocity.SetVelocity( Vector3.zero );

				collision = true;

				Debug.Log( "hit : "+rayHit.point, rayHit.transform );
			}
		}

		// Apply our final position to the object
		transform.position = nextPosition;

		// we must update the pan and pancake last so that the position has been updated.
		// prevent the objects geting the position.
		if( collision )
		{
			// Update pancake on pan and pan on pancake.
			SendMessages( rayHit.transform, GetComponent<Pancake_state>() );

		}

	}

	//TODO: I need real name. (Im scared, i dont know what i am )
	private void c()
	{
		pancake_velocity.PhysicsStep( Time.deltaTime );

		Vector3 nextPosition = transform.position + pancake_velocity.GetTravleDistance( Time.deltaTime );

		transform.position = nextPosition;

	}

	public void TransformToUpforce(Vector3 forwardsDirection, float distance, float yRotation )
	{
		// only want to transform to upforce for the point that is furthest away
		if ( panColliderObj == null && distance < transformUpforceDistance ) return;


		float vel = pancake_velocity.Velocity.x + pancake_velocity.Velocity.z;

		if ( vel < upforceThresshold )
			return;

		SendMessages( null, null );
		SendMessage( "SetFlipRotation", yRotation );
		//pancake_velocity.SetVelocity( /*new Vector3(0, vel, 0) );/*/ forwardsDirection * vel );//*/
		transformedVelocity = forwardsDirection * vel;
	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;

		if ( panColl != null )
			positionInPan = panColl.InverseTransformPoint( transform.position );
	}

	public Transform GetPanCollider()
	{
		return panColliderObj;
	}

	void SendMessages(Transform panCollider, Pancake_state state)
	{

		IPanCollider[] panCols = GetComponents<IPanCollider>();
		IReceivePancake[] recivePancakes = GetComponentsInParent<IReceivePancake>();

		int i = 0;

		while ( i < panCols.Length || i < recivePancakes.Length)
		{

			if ( i < panCols.Length )
				panCols[ i ].SetPanCollider( panCollider );

			if ( i < recivePancakes.Length )
				recivePancakes[ i ].AddPancake( state );

			i++;
		}

	}

}
