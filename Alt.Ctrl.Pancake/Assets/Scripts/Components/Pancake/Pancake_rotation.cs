using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pancake_velocity))]
public class Pancake_rotation : MonoBehaviour, IPanCollider
{
	private Transform panColliderObj;
	private Pancake_velocity velocity;

	private Transform rotateObj;

	[SerializeField] private float rotateSpeed = 180f; // per second
	private float flipSpeed = 0;
	[Tooltip("When the rotate speed drops below the delta thresshold it will attampt to level the pancake out to next 180 degree (ie. 0/360 or 180")]
	[SerializeField] private float flipSpeedDeltaThresshold = 5f;
	private float targetFinishRotation = -1;				  // <0 is unset. 

	private void Awake()
	{
		rotateObj = new GameObject().transform;
	}

	// Start is called before the first frame update
	void Start()
    {

		velocity = GetComponent<Pancake_velocity>();

    }

    // Update is called once per frame
    void Update()
    {

		if ( panColliderObj != null ) return;   // dont rotate if we are in the pan.
		if ( velocity.GetVelocity().y == 0 ) return;

		Vector3 currentVelocity = velocity.GetVelocity();

		// Work out the flip speed.
		// level the pancake out to the next 180degrees if the pancake is falling or below the flipspeed thresshold
		// else work out the flip speed by the pancakes velocity.
		// This will give us a dynamic flip velocity and will make it less likely that the pancake will land verticaly.
		// giving the player a better chance of compleating a flip :)

		// fs < 0 ??
		if ( flipSpeed < 0f && ( currentVelocity.y < 0 || flipSpeed > -flipSpeedDeltaThresshold ) )
		{
			if ( targetFinishRotation < 0 )
				UpdateTargetFinishRotation();

			// find the differeance between our current rotation and targetFinishRotation.
			float targetDif = targetFinishRotation - AMS_Helpers.Maf.ClampRotation(rotateObj.eulerAngles.z);

			// we must make sure that we do NOT exceed the current flipSpeed. 
			// we do not want the pancakes rotation to randomly speed up
			if ( -targetDif < flipSpeed )
				flipSpeed -= flipSpeed * Time.deltaTime;		// slowly decress the flip speed so it can come to a hult.
			else
				flipSpeed = -targetDif;                          // this will naturally slow down, the closer we get to the target.

			print( "goingToTarget" );

		}
		else if ( currentVelocity.y > 0f )
		{
			flipSpeed = ( currentVelocity.y / ( ( Mathf.Abs( currentVelocity.x ) + Mathf.Abs( currentVelocity.z ) ) / 2f ) ) * rotateSpeed; //per sec;
			print( "fliping ## cv: "+ currentVelocity.y +" /  x "+  Mathf.Abs( currentVelocity.x ) + " ## z" +Mathf.Abs( currentVelocity.z ) +" ## x/z "+ ( ( Mathf.Abs( currentVelocity.x ) + Mathf.Abs( currentVelocity.z ) ) / 2f ) );

		}

		print( " FlipSpeed: " + flipSpeed );
		// move the pancake into the rot object and rotate around the zAxis, and remove pancake from rotate object.
		// this alows us to rotate the pancane in the same direction as the force when leaving the pan :)
		// The rotate object is oriantated when the flip starts in SetFlipRotation :D

		Vector3 nextRot = rotateObj.eulerAngles;
		nextRot.z += flipSpeed * Time.deltaTime;

		// make sure the rotateObj is in the same position as the pancake befor makeing the pancake a child
		// so we always rotating from the center...
		rotateObj.position = transform.position;

		transform.parent = rotateObj;
		rotateObj.eulerAngles = nextRot;

		transform.parent = null;
		
		// TODO: level flip off when below flipspeed thresshold?? DONE??

    }

	public void SetPanCollider( Transform panCollObj )
	{
		panColliderObj = panCollObj;
	}

	public void SetFlipRotation()
	{

		Vector3 velNorm = velocity.Velocity.normalized;
		// flip the x/z axis since the bones are orineted so that x is forwards :|
		// and set y to zero so we dont look up and down :)
		velNorm.x = velocity.Velocity.normalized.z;
		velNorm.y = 0;
		velNorm.z = velocity.Velocity.normalized.x;

		rotateObj.LookAt( rotateObj.position + velNorm );

		// reset the target rotation, so we know it needs to be work out again once we are below the flip thresshold or falling.
		targetFinishRotation = -1;
		flipSpeed = 0;

	}

	private void UpdateTargetFinishRotation()
	{

		// find the next rotation that is a mulitple of 180 (ie 0/360 or 180)

		// TODO: take the current flip speed into account. if it is above the thresshold (this is posible if we start falling befor the thresshold is reached)
		// the target finish should be the mulitple of 180 affter the next.

		// P.S we only rotation on the Z axis. :)

		float currentZRotation = AMS_Helpers.Maf.ClampRotation( rotateObj.eulerAngles.z );

		// Find how far past the last multiple of 180 we are so it can be taken away from the current rotation 
		// then add 180 for the next mutilple of 180 :)
		float remainingRotation = currentZRotation % 180;

		targetFinishRotation = currentZRotation - remainingRotation + 180f;
		
	}

	private void OnDestroy()
	{
		// destroy rotate object, we dont want a scene full of empty GO's
		Destroy(rotateObj.gameObject);
	}

}
