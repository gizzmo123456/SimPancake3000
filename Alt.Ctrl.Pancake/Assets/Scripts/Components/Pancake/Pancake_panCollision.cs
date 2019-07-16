using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: rename this class, collision is only one of the things it deals with
[ RequireComponent( typeof( Pancake_velocity	) ) ]
[ RequireComponent( typeof( Pancake_state		) ) ]
[ RequireComponent( typeof( Pancake_jointSetup	) ) ]	// required to access the array of pancake joints.
public class Pancake_panCollision : Raycast_hit, IPanCollider, IChild
{

	private Pancake_state state;
	private Pancake_velocity pancake_velocity;
	private Pancake_joint[] pancake_joints;

	private Transform panColliderObj;
	/*private*/ public Vector3 positionInPan;          // the local position of the pancake in the pan

	[Header( "Collider things" )]
	[Range(0f, 2f)]
	[SerializeField] private float panFriction = 0.25f;
	[SerializeField] private AnimationCurve panFrictionCurve;
	private float panFriction_distanceFromCenter = 0f;

	[SerializeField] private float airFriction = 0.1f;

	[SerializeField] private float upforceThresshold = 1f;
	private float transformUpforceDistance = 0;
	private Vector3 transformedVelocity = Vector3.zero;

	// [Header( "Collider/Ray things" )]
	private int ray_jointId = 0;
	protected override Vector3 RayPosition { get { return pancake_joints[ray_jointId].transform.position; } }

	private bool isChild = false;

	void Awake()
	{

		pancake_velocity = GetComponent<Pancake_velocity>();
		state = GetComponent<Pancake_state>();
		
	}

	private void Start()
	{
		// must get joints in start to make shore the joints and array have been initilized
		pancake_joints = GetComponent<Pancake_jointSetup>().GetJoints();    // NOTE: I dont think we need all the joints, rather only the outer joints, and maybe the center for safety.
		panFriction_distanceFromCenter = GetComponent<Pancake_jointSetup>().GetMaxDistanceFromCenter();
	}

	private void /*Fixed*/Update()
	{

		if ( isChild ) return;	// the pancake will move with its parent. (see pancake_child)

		// Apply Transformed upforce and send messages to clear it from the pan.
		if(transformedVelocity != Vector3.zero)
		{

			pancake_velocity.SetVelocity( transformedVelocity );
			pancake_velocity.SetVelocity( Vector3.zero, Pancake_velocity.VelocityType.Limited );		// clear the limited velocity that is used for slinding in the pan.
			transformedVelocity = Vector3.zero;
			SendMessages( null, null );

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
		// for now i can just cowboy it and let say we're on side 0 the offset is x or y is 0 else x or y offset is 180??
		// TODO: make the rotation of the pancake match the rotation is landed in the pan.
		// i think i wil have to work out the rotation dif, but not to sure at the moment.
		Vector3 rotation_sideOffset = state.GetSideDown() == 0 ? Vector3.zero : new Vector3(0, 0, 180);		// would it be better if i just inverted the scale??...
		transform.eulerAngles = panColliderObj.eulerAngles + rotation_sideOffset; // TODO: needs to make more dynamic ^^^

		positionInPan += pancake_velocity.GetTravleDistance( Time.deltaTime );
		transform.localPosition = panColliderObj.TransformPoint( positionInPan );

		float panFrictionMutiplier = panFrictionCurve.Evaluate( Vector3.Distance( Vector3.zero, positionInPan ) / panFriction_distanceFromCenter );

		pancake_velocity.AddFriction( panFrictionMutiplier * panFriction, panFrictionMutiplier * panFriction );

	}

	//TODO: can i also get a real name. (awwwwwwwwwwww, im falling...)
	private void b()
	{
		// Apply the physic step now so we can work out where we will be at the end of this update.
		pancake_velocity.PhysicsStep( Time.deltaTime );
		pancake_velocity.AddFriction( airFriction, panFriction );

		// find the max distance that we are going to move during the update.
		Vector3 nextPosition = transform.position + pancake_velocity.GetTravleDistance( Time.deltaTime );

		JointHitData hitData = ResolveJointCollisions();    // find if any collision occurred, null if none.
		bool collision = hitData != null;

		if ( collision )
		{
			// TODO: 
			// For the moment just going to move the pancake's transform the same distance the joint has moved :)
			Vector3 jointDistanceTraved = pancake_joints[hitData.jointId].transform.position - hitData.hitPosition;
			Debug.LogWarning( "BipBop"+ jointDistanceTraved + " # po: "+ transform.position +" # npo: "+ transform.position + jointDistanceTraved +" # Vel: "+ pancake_velocity.Velocity.y );

			nextPosition = transform.position + jointDistanceTraved;
			pancake_velocity.SetVelocity( Vector3.zero, true );
		}

		// Apply our final position to the object
		transform.position = nextPosition;

		// we must update the pan and pancake last so that the position has been updated.
		if( collision )
		{
			// Update pancake on pan and pan on pancake.
			SendMessages( hitData.hitObject, GetComponent<Pancake_state>() );

		}

	}

	//TODO: I need real name. (Im scared, i dont know what i am )
	private void c()
	{

		pancake_velocity.PhysicsStep( Time.deltaTime );
		pancake_velocity.AddFriction( airFriction, panFriction );

		Vector3 nextPosition = transform.position + pancake_velocity.GetTravleDistance( Time.deltaTime );

		transform.position = nextPosition;

		Debug.LogWarning( " # Vel: " + pancake_velocity.Velocity.y );


	}

	/// <summary>
	/// Find if any joints collid with an object
	/// </summary>
	/// <returns> null if no collision occurred </returns>
	private JointHitData ResolveJointCollisions()
	{

		// NOTE: starting basic!! :D
		// NOTE: remember to remove note ^^^
		// cast the ray from all joints and find the collest collision point.

		// TODO: stop overscaning. its pointless.
		Vector3 distanceTravled = pancake_velocity.GetTravleDistance( Time.deltaTime );
		distance = Vector3.Distance( Vector3.zero, distanceTravled );

		Vector3 jointNextPosition = Vector3.zero;

		JointHitData hitData = new JointHitData();
		float minDistance = -1;	// <0 not inited

		for (int i = 0; i < pancake_joints.Length; i++ )
		{
			// update the current joint and fire the ray
			ray_jointId = i;

			if(CastRay())
			{
				// Find if this is the cloest point.
				jointNextPosition = pancake_joints[ i ].transform.position + distanceTravled;

				if(jointNextPosition.y <= rayHit.point.y) // collision.
				{

					float dist = Vector3.Distance( pancake_joints[ i ].transform.position, jointNextPosition );

					if( minDistance < 0 || dist < minDistance)
					{
						minDistance = dist;

						// set the new cloest data.
						hitData.jointId = i;
						hitData.hitPosition = rayHit.point;
						hitData.hitObject = rayHit.transform;

					}

				}

			}
			
		}

		// if the min distance is still less than 0, no collision has occurred
		return minDistance < 0 ? null : hitData;

	}

	/// <summary>
	/// Transform the current velocity into flip velocity.
	/// </summary>
	/// <param name="forwardsDirection"> the direction to move forwards in </param>
	/// <param name="distance"> distance from center of pan</param>
	/// <param name="yRotation"> y rotation of point </param>
	/// <param name="zPosition"> z position of point</param>
	public void TransformToUpforce(Vector3 forwardsDirection, float distance, float zPosition )
	{
		// only want to transform to upforce for the point that is furthest away
		// and it must be in the front of the pan (+z)
		// you cant realy flip form the back of a frying pan, that would be black magic.
		// BUG: Here where we test for zPos > this.pos.z is checking to see if pancake is in the front half of the pan.
		//		But we are check if the joint is in the front half of the pan insed. the prob is that the joint can be in the 
		//		Front half of the pan where as the pancake center it self can be in the back half of the pan.
		//		Thurermore the is causing the pancake to flip altho it in the wrong position.
		//		and it might be contubuting the sticking.
		// TODO: Move to Can Flip
		if ( transform.position.z < panColliderObj.position.z && zPosition > panColliderObj.position.z && transformedVelocity != Vector3.zero && distance < transformUpforceDistance ) return;

		if ( Pancake_DEBUG.debug_joints )
			print( "Accepting next..."+(panColliderObj == null)+" && "+(distance < transformUpforceDistance) );


		float vel = /*pancake_velocity.Velocity.sqrMagnitude;/*/ pancake_velocity.Velocity.x + pancake_velocity.Velocity.z;

		if ( vel < upforceThresshold )
			return;

		transformUpforceDistance = distance;

		SendMessage( "SetFlipRotation");	// should be in the if statment ??

		// correct the up direction when the pancake has been flips
		if ( state.GetSideDown() == 1 )
			forwardsDirection.y = -forwardsDirection.y;

		// It is worth noting that we do NOT need to take the pancakes mass into account when we transform the pancakes velocity to up/flip
		// since its mass has been taken into account up until now and we are only transforming it current velocity into it upforce.
		// As it stands we do NOT add any velocity to aid the fliping process (altho that might change)
		// so taking that all into account we must use SetVelocity witch ignores the pancake mass :)

		// we set the velocity in Update function :) as each joint need to call this function to find if its the thurthest away.
		// so we can not update the velocity until all joints have been tested. so we'll just set the velocity at the start of the next update.
		// not ideal but hay...
		// ... TODO: unpdate the joints in here ??? indted of the joint calling this function. 
		transformedVelocity = forwardsDirection * vel;

#if UNITY_EDITOR
		if ( transformedVelocity.y < 0f )
		{
			Debug.Log( "I Paused The Editor :D If this has happened check the joints havent folded over them selfs again. Oh and TransformedVelocity.y is < 0 btw" );
			UnityEditor.EditorApplication.isPaused = true;
		}
# endif

	}

	public bool CanFlip()
	{

		return false;
	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;

		if ( panColl != null )
			positionInPan = panColl.InverseTransformPoint( transform.position );	// BUG: ?? for an unkown reason when the pancake re-enters the pan it can somtimes snap to the center.
	}

	public Transform GetPanCollider()
	{
		return panColliderObj;
	}

	public void SetPositionInPan( Vector3 position, bool isLocal, bool update )
	{

		if ( isLocal )
			positionInPan = position;
		else
			positionInPan = panColliderObj.InverseTransformPoint( position );

		if(update)
			transform.position = panColliderObj.TransformPoint( positionInPan );

	}

	void SendMessages(Transform panCollider, Pancake_state state)
	{

		IPanCollider[] panCols = GetComponents<IPanCollider>();
		IReceivePancake[] recivePancakes = GetComponentsInParent<IReceivePancake>();
		FryingPan_pancake fryingpan = null;

		// add / remove pancake from frying pan.
		// if state is null then we are removing so we will need to get the pancake_state from this object. :)
		if ( state == null )
		{
			// attamp to get the fryingpan_pancake from the pan that we are leaving
			// befor it gets removed.
			if ( panColliderObj )
				fryingpan = panColliderObj.GetComponentInParent<FryingPan_pancake>();

			fryingpan?.RemovePancake( GetComponent<Pancake_state>() );
		}
		else
		{
			// attamp to get the fryingpan_pancake from the pan that we are entering
			// befor it gets added.
			if ( panCollider )
				fryingpan = panCollider.GetComponentInParent<FryingPan_pancake>();

			fryingpan?.AddPancake( state );
		}
		

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

	public void SetIsChild( bool isChi )
	{
		isChild = isChi;
	}

	class JointHitData
	{
		public int jointId;
		public Transform hitObject;
		public Vector3 hitPosition;

		public JointHitData()
		{
			jointId = 0;
			hitObject = null;
			hitPosition = Vector3.zero;
		}

		public JointHitData( int jId, Transform hitObj, Vector3 hitPos)
		{
			jointId = jId;
			hitObject = hitObj;
			hitPosition = hitPos;
		}

	}

}
