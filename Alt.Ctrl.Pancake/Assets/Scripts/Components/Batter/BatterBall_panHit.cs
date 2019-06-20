using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Batter_quantity))]
public class BatterBall_panHit : Raycast_hitVelocityDistance, IBatterRelease
{

	private bool updating = false;

	protected override void FixedUpdate()
	{
		
		if ( !updating ) return;

		base.FixedUpdate();

		if( CastRay() )
			OnPanHit();
	
	}

	private void OnPanHit( )
	{
		
		// check that we have reached the pan. 
		// the length of the raycast can be longer than the travel distance.
		if ( NextPosition.y > rayHit.point.y ) return;

		// correct it position, turn off gravity and the raycast since we have now reached the pan, and we wont be colliding with anythink else.
		updating = false;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = rayHit.point;


		//Attamp to find the fryingPan Pancake class, it has to search its parents since the collider is nested into the object.
		FryingPan_pancake panPancake = rayHit.transform.GetComponentInParent<FryingPan_pancake>();

		if ( panPancake == null ) return;

		// Get batter qt of the batterBall and apply it to a mixture pancake.
		Batter_quantity quantity = GetComponent<Batter_quantity>();

		panPancake.AddBatter( quantity.GetBatterQuantity(), rayHit.point );
		transform.position = rayHit.point;

	}

	public void OnBatterRelease()
	{	// we only need update once we have been released for the jug :)
		updating = true;
	}
}
