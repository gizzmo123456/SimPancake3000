using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// it might be an idear to rename this class.
// since it will now deal with all pancake forces :)

/*	Read Note Above^^
 *  Handles the pancakes forces (includeing flip)
 *  
 */
 [RequireComponent(typeof(Rigidbody))]
public class PancakeFlip : MonoBehaviour
{

	private FryingPan fryingPan;
	[SerializeField] private Vector3 targetReturnPosition;
	private Rigidbody rigid;
	[SerializeField] private Pancake_panDetect frw_panDetect;

	public bool debug = false;

	private float force = 0;
	private float forceY = 0;

	private Vector3 velocity = Vector3.zero;

	[SerializeField] private float counterForce = 2f;
	private int counterDir = 0;

	[SerializeField] private float z_transferRate = 2; // can be removed
	[SerializeField] private float z_counter = 2;
	private bool zReturn = false;

	[SerializeField] private float yVelMutiply = 0.75f;
	[SerializeField] private float zVelMutiply = 0.2f;

	private bool applyingCounterForce = false;

	private float disPercent;
	[SerializeField] private float upDistance = 0.4f;
	[SerializeField] private AnimationCurve upCurve;

	private void Awake()
	{

		rigid = GetComponent<Rigidbody>();

	}

	public void SetFryingPan( FryingPan pan )
	{
		fryingPan = pan;

		// reset the velocity when entering in the frying pan
		// TODO: maybe we could make this a lil more dynamic
		if ( pan != null )

			//velocity = Vector3.zero;

		force = forceY = 0;

	}

	private void FixedUpdate()
	{
		
	}

	private void FixedUpdate_old()
	{

		TranslateForce();

		if ( velocity.y < 0 )
			velocity.y = rigid.velocity.y;

		// Add Velocity
		rigid.velocity = velocity;

		if ( fryingPan != null ) return;

		// work out next frames velocity
		print( "Im out" );
		velocity.y += Physics.gravity.y * Time.deltaTime;

		if(counterDir >= 0 && velocity.z > 0)
		{
			counterDir = 1;
			velocity.z -= counterForce * Time.deltaTime;
		}
		else if(counterDir <= 0 && velocity.z < 0) 
		{
			counterDir = -1;
			velocity.z += counterForce * Time.deltaTime;
		}
		
		if( (counterDir < 0 && velocity.z > 0 ) || (counterDir > 0 && velocity.z < 0) )
		{
			velocity.z = 0;
			counterDir = 0;
		}

		//lock vel x for the time being
		velocity.x = 0;

	}

	private void FixedUpdate_old_old()
	{

		if ( velocity == Vector3.zero )
			return;

		Vector3 rbVelocity = rigid.velocity;
		velocity.x = 0;

		// if we are falling ignore velocity and let unity deal with it :)
		if ( velocity.y <= 0 )
			velocity.y = rbVelocity.y;

		rigid.velocity = new Vector3( 0, velocity.y, velocity.z );
		transform.LookAt( transform.position + velocity );              // TODO make own lookat so it can flip over more then once (but this will do for now)

		// work out the velocity ready for the update.

		float targetReturnForce_z = ( targetReturnPosition.z - transform.position.z ) * ( 1f / Time.deltaTime );// + velocity.z; // i could precompute the frame rate since its in fixed update

		if ( !applyingCounterForce && targetReturnForce_z < 0 && velocity.z >= targetReturnForce_z )
		{
			
			//velocity.y -= (targetReturnForce_z - velocity.z) * yVelMutiply * Time.deltaTime;
			velocity.z += ( targetReturnForce_z - velocity.z ) * zVelMutiply * Time.deltaTime;
			//velocity.z -= z_counter * Time.deltaTime;
			/*
			//im thinking maybe do somthink like Vz = ((cf-tf)/2f) + tf;
			float vz = ( ( velocity.z - targetReturnForce_z ) / 2f ) + ( targetReturnForce_z * zVelMutiply );
			velocity.y += (velocity.z - vz) * yVelMutiply * Time.deltaTime;
			velocity.z += vz * Time.deltaTime;
			print( ( ( velocity.z - targetReturnForce_z ) / 2f ) + " + " + ( targetReturnForce_z * zVelMutiply ) );
			*/


		}
		else if( targetReturnForce_z > 0 || velocity.z < targetReturnForce_z )
		{
			// if we have left the pan make shore we can not return to the prv state. 
			if ( fryingPan == null ) applyingCounterForce = true;
			if(debug)
				UnityEditor.EditorApplication.isPaused = true;

			velocity.z += z_counter * Time.deltaTime;

			if ( velocity.z > 0 )
				velocity = Vector3.zero;
		}
		else
		{
			Debug.LogError("I'm Brocken!");
		}

		if(velocity != Vector3.zero)
			velocity.y += Physics.gravity.y * Time.deltaTime;               // i could precompute this sum, since its in the fixed update.

		//	print("########### TRP: "+targetReturnPosition+" ## P: "+transform.position+" ## TRFz: "+ targetReturnForce_z +" ("+ ( targetReturnPosition.z - transform.position.z ) +" * "+ ( 1f / Time.deltaTime ) + ") ## TRFzV: "+(targetReturnForce_z - velocity.z)+" ## v: "+velocity);
		TranslateForceUp();
	}

	void FixedUpdate_old_old_old()
    {

		if ( /*fryingPan == null ||*/ velocity == Vector3.zero )
		{
			//transform.eulerAngles = new Vector3( 0, 90, 0 );
			return;
		}

		float rbVel_y = rigid.velocity.y;

		if ( velocity.y < 0 )
			velocity.y = rbVel_y;

		rigid.velocity = new Vector3(0, velocity.y, velocity.z * zVelMutiply); 
		transform.LookAt( transform.position + velocity );

		if ( velocity.z > 0 )
			velocity.y += z_transferRate * Time.deltaTime;

		/*
		else
			velocity.y -= z_transferRate * Time.deltaTime;
		*/
		if(!zReturn)
			velocity.z -= z_transferRate * Time.deltaTime;
		else
			velocity.z += z_counter * Time.deltaTime;

		velocity.y += Physics.gravity.y * Time.deltaTime;

		if ( velocity.z < -50 && !zReturn )
			zReturn = true;
		else if ( zReturn & velocity.z > 0 )
			velocity = Vector3.zero;


		velocity.x = 0;
    }

	public void AddPancakeForce( Vector2 f )// float f, float fy )// Vector3 force)
	{
		/*
			if ( velocity == Vector3.zero )
				targetReturnPosition = transform.position;
		*/
		// It might be an idear to have force as a float to simlifi this a lil
		// but i think it might be an idear to move some of the forwards force to up.
		// ATM this happens in fryingPan.ApplyForceToPancake() 
		//velocity += force;

		force += f.x;
		forceY += f.y;

	}

	/// <summary>
	/// translate the current force up in fryingpan space :)
	/// </summary>
	private void TranslateForceUp()
	{
		Vector3 projectedForce = Vector3.Project(velocity, fryingPan.transform.up*-10);

		print( "v: " + velocity + " ## pv: " + projectedForce +" ## up: "+fryingPan.transform.up +" ## Rz: "+fryingPan.transform.eulerAngles.z);
	}

	

	private void TranslateForce()
	{
		if ( fryingPan == null )
			return;

		disPercent = Mathf.Clamp01((frw_panDetect.HitDistance-0.35f)/upDistance);

		velocity = Vector3.Lerp( fryingPan.transform.up , fryingPan.transform.right, upCurve.Evaluate(disPercent) ) * force;
		/*
		if ( frw_panDetect.HitDistance >= 0 && frw_panDetect.HitDistance < 0.3 )
		{
			velocity = fryingPan.transform.up * force;
			fryingPan = null; //<<-- im not getting pissed off
		}
		else
		{
			velocity = fryingPan.transform.right * force;
		}
		*/
		//	velocity.y += forceY; 

		print( "Vel: " + velocity + " ## distance: " + frw_panDetect.HitDistance +" ## force: "+force+" ## % "+ disPercent );

	}

}
