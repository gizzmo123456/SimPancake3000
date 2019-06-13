using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePoint : MonoBehaviour
{

	[SerializeField] private Rigidbody[] velocityPoints;
	int activeCount = 0;

	[SerializeField] private float force = 0;
	[SerializeField] private float counterForce = 0.5f;
	Vector3 velocity = Vector3.zero;

	[SerializeField] private Transform directionTransform;
	[SerializeField] private Vector3 direction = new Vector3( 0, 0, 1 );

	private bool isInPan = false;
	[SerializeField] private float returnForce = 0.125f;

	private void Start()
	{
		// Attampt to bind onto the distance active callback
		foreach (Rigidbody rb in velocityPoints)
		{
			DistanceFrom df = rb.GetComponent<DistanceFrom>();
			if( df != null )
			{
				df.ActiveDistance += DistanceActive;
			}
		}
	}

	private void FixedUpdate()
	{

		if ( !isInPan ) return;

		velocity = GetVelocity();

		if ( velocity == Vector3.zero ) return;
		
		foreach ( Rigidbody rb in velocityPoints )
			rb.AddForce(velocity, ForceMode.Impulse);

		print( "AddForce" );
		
		force -= counterForce * Time.deltaTime; 

		if ( force < 0 ) force = 0;

	}

	private Vector3 GetVelocity()
	{

		if ( force <= 0 ) return Vector3.zero;

		Vector3 direct = Vector3.zero;

		if (activeCount == 0)
		{
			direct = directionTransform.TransformDirection( direction );
		}
		else
		{
			direct = directionTransform.up + new Vector3(0, 0, returnForce);
		}

		return direct * force;

	}

	void DistanceActive(bool active, DistanceFrom distFrom)
	{
		activeCount += active ? 1 : -1;
	}

	void SetPan(bool inPan)
	{
		isInPan = inPan;

		if ( !inPan )
		{
			force = 0;
			
		}

	}
}
