using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores object without rigidbodys, velocitys
/// </summary>
[RequireComponent( typeof( Batter_quantity ) )]
public class Pancake_velocity : MonoBehaviour, IVelocity
{
	// Note: should this have both world a local velcoity.
	// or at least a method to convert to and from local velocity of anouther object.
	// or maybe it could have a method to switch in and out of a velocity mode (local and world) <<-- i think that might be better.

	public enum VelocityType { Default, Limited }

	private bool physicsEnabled = true;
	private Batter_quantity batterQuantity;
	[SerializeField] private Transform updateSpace;         // space that velocity is oriented to. if null then world space. //TODO:	// DOES this even get used??
	[SerializeField] private Vector3 physicFriction = Vector3.zero;
	[Tooltip("Keep values positive. negative values can cause unexpected issues. the limited velocity range is '-limit, limit' ")]
	[SerializeField] private Vector3 limitedVelocity_limit = Vector3.one;


	private Vector3 velocity = Vector3.zero;
	private Vector3 limitedVelocity = Vector3.zero;

	public Vector3 Velocity { get { return velocity + limitedVelocity; } }	// Not sure if this should include the limited velocity.
	private float Mass { get { return batterQuantity.Mass; } }	// NOTE: it might be better if this was set by Pancake scale, but i want to play factorio, so this will do for now.

	private void Awake()
	{
		batterQuantity = GetComponent<Batter_quantity>();
	}

	/// <summary>
	/// Set the velocity of a pancake take its mass into acount.
	/// </summary>
	/// <param name="vel">Velocity to add</param>
	public void AddVelocity( Vector3 vel )
	{
		velocity += vel * Mass;
	}

	public void AddVelocity( Vector3 vel, VelocityType type )
	{

		if(type == VelocityType.Default)
		{
			AddVelocity( vel );
			return;
		}

		// Add limited velocity
		limitedVelocity += vel;
		limitedVelocity = limitedVelocity.Clamp( -limitedVelocity_limit, limitedVelocity );

	}

	/// <summary>
	/// Set the velocity of a pancake ignoring the mass.
	/// </summary>
	/// <param name="vel">Velocity to add</param>
	public void SetVelocity( Vector3 vel )
	{
		SetVelocity( vel, false );
	}

	/// <summary>
	/// Set the velocity of a pancake ignoring the mass.
	/// </summary>
	/// <param name="clearLimitedVelocity">Should the limited velocity be cleared</param>
	/// <param name="vel">Velocity to add</param>
	public void SetVelocity( Vector3 vel, bool clearLimitedVelocity )
	{
		velocity = vel;

		if ( clearLimitedVelocity )
			limitedVelocity = Vector3.zero;

	}

	public void SetVelocity( Vector3 vel, VelocityType type)
	{

		if (type == VelocityType.Default)
		{
			SetVelocity( vel);
			return;
		}
		
		limitedVelocity = vel;
		limitedVelocity = limitedVelocity.Clamp( -limitedVelocity_limit, limitedVelocity_limit );

	}

	/// <summary>
	/// Gets the velocity scaled to update space. world if null.
	/// </summary>
	/// <param name="scaleTrans">if surplied scales velocity to transform (default: null)</param>
	/// <returns>velocity scaled to transform, in no transform them returns velocity</returns>
	public Vector3 GetVelocity()
	{

		if ( updateSpace )
			return updateSpace.TransformDirection(velocity + limitedVelocity);	//mige be better to use Vector insted of direction
		else
			return velocity + limitedVelocity;

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
		if ( !physicsEnabled ) return;

		velocity += Physics.gravity * delta;

		PhysicsFriction();

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

	/// <summary>
	/// Takes the pancakes rotation into account.
	/// </summary>
	private void PhysicsFriction()
	{

		Vector3 friction = Vector3.zero;

		// dont need the Y axis, it just spins thus does not affect friction
		Vector3 mod180 = Vector3.zero;
		mod180.x = transform.eulerAngles.x % 180f;
		mod180.z = transform.eulerAngles.z % 180f;

		// By adding a 90 deg offset to the mod180 and takeing 180 we get the rotation in a rage of -90 & 90, 0 being 90 deg.
		// so if do this for the X and Z frict we can then chose the lowers ABS value for the Y (vert) Friction (90), then all we have 
		// to do is take friction value for X/Z away from 90 for the hoz friction (180).

		friction.x = Mathf.Abs( mod180.x + 90f - 180f );
		friction.z = Mathf.Abs( mod180.z + 90f - 180f );

		// find the lowest for the Y friction
		// TODO: maybe we could make this better than lowest. Projection ?
		friction.y = friction.x < friction.z ? friction.x : friction.z;

		// take the friction from 90 for the hoz friction (180)
		friction.x = 90f - friction.x;
		friction.z = 90f - friction.z;

		// turn it into a percentage.
		friction.x /= 90f;
		friction.y /= 90f;
		friction.z /= 90f;

		// and finaly mutiple it by the physics friction :D
		friction = friction.Multiply( physicFriction );

		AddFriction( ref velocity, friction );

	}

	/// <summary>
	/// Add friction to both the velocity and limited valocity. 
	/// only affecting X and Z axis.
	/// </summary>
	/// <param name="velocityFriction"></param>
	/// <param name="limitedVelocityFriction"></param>
	public void AddFriction( float velocityFriction, float limitedVelocityFriction )
	{

		AddFriction( ref velocity, velocityFriction );          // friction for default velocity
		AddFriction( ref limitedVelocity, velocityFriction );   // friction for limited velocity

	}

	/// <summary>
	/// Add friction to X/Z axis only.
	/// </summary>
	/// <param name="vel"></param>
	/// <param name="friction"></param>
	private void AddFriction( ref Vector3 vel, float friction )
	{

		// aka counter force x, z
		vel.x = StopAtZero( vel.x, friction * Time.deltaTime );
		vel.z = StopAtZero( vel.z, friction * Time.deltaTime );

	}

	/// <summary>
	/// Added friction to each axis (x, y, z)
	/// </summary>
	/// <param name="vel"></param>
	/// <param name="friction"></param>
	private void AddFriction( ref Vector3 vel, Vector3 friction )
	{

		vel.x = StopAtZero( vel.x, friction.x * Time.deltaTime );
		vel.y = StopAtZero( vel.y, friction.y * Time.deltaTime );
		vel.z = StopAtZero( vel.z, friction.z * Time.deltaTime );

	}

	/// <summary>
	/// If value is grater than zero value 2 is taken away. if value is less than zero value is added. WILL NOT cross over zero.
	/// </summary>
	private float StopAtZero(float value, float value2) // what the hell do you call a function that does this.
	{

		if( value < 0 )
		{

			value += value2;

			if ( value > 0 )
				value = 0;

		}
		else if( value > 0)
		{

			value -= value2;

			if ( value < 0 )
				value = 0;
			
		}

		return value;

	}

	public void EnabledPhysics( bool enable )
	{
		physicsEnabled = enable;
	}
}
