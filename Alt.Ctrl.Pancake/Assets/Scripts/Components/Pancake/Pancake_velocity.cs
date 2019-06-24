using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores object without rigidbodys, velocitys
/// </summary>
public class Pancake_velocity : MonoBehaviour, IVelocity
{
	// TODO: should this have both world a local velcoity.
	// or at least a method to convert to and from local velocity of anouther object.
	// or maybe it could have a method to switch in and out of a velocity mode (local and world) <<-- i think that might be better.

	[SerializeField] private Transform updateSpace;         // space that velocity is oriented to. if null then world space. //TODO: 

	private Vector3 velocity = Vector3.zero;
	public Vector3 Velocity { get { return velocity; } }

	public void AddVelocity( Vector3 vel )
	{
		velocity += vel;
	}

	public void SetVelocity( Vector3 vel )
	{
		velocity = vel;
	}

	/// <summary>
	/// Gets the velocity scaled.
	/// </summary>
	/// <param name="scaleTrans">if surplied scales velocity to transform (default: null)</param>
	/// <returns>velocity scaled to transform, in no transform them returns velocity</returns>
	public Vector3 GetVelocity()
	{

		if ( updateSpace )
			return updateSpace.TransformDirection(velocity);	//mige be better to use Vector insted of direction
		else
			return velocity;

	}

	public Vector3 GetTravleDistance(float delta)
	{

		return GetVelocity() * delta;

	}

	/// <summary>
	/// add gravity to velocity for delta amount of time
	/// </summary>
	/// <param name="delta">Amount of time since the last physic step or update</param>
	public void PhysicsStep(float delta)
	{

		velocity += Physics.gravity * delta;

	}

	public void SetUpdateSpace( Transform spaceTrans )	//NOTE: dont know if this is being called from anywhere, but all seems to be working just fine :)
	{
/*
		// convert from the current space to the new space.

		// convert to world space if we are already in another objects local space
		if (updateSpace != null)
			velocity = updateSpace.TransformVector(velocity);


		// convert from world to the new local space (if passed in).
		if ( spaceTrans != null )
			velocity = spaceTrans.InverseTransformVector( velocity );
*/		

		updateSpace = spaceTrans;

	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns> in world space if no update space object</returns>
	private Vector3 TransformVectorToLocal( Vector3 worldPosition)
	{
		if ( updateSpace == null ) return worldPosition;
		else return updateSpace.InverseTransformVector( worldPosition );
	}

	public void AddFriction( float friction )
	{

		// aka counter force x, z
		if ( velocity.x < 0 )
		{
			velocity.x += friction * Time.deltaTime;
			if ( velocity.x > 0 ) velocity.x = 0;
		}
		else if ( velocity.x > 0 )
		{
			velocity.x -= friction * Time.deltaTime;
			if ( velocity.x < 0 ) velocity.x = 0;
		}

		if ( velocity.z < 0 )
		{
			velocity.z += friction * Time.deltaTime;
			if ( velocity.z > 0 ) velocity.z = 0;
		}
		else if ( velocity.z > 0 )
		{
			velocity.z -= friction * Time.deltaTime;
			if ( velocity.z < 0 ) velocity.z = 0;
		}

	}

}
