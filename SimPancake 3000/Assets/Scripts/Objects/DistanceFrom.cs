using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFrom : MonoBehaviour
{
	public delegate void activeDistance(bool active, DistanceFrom fromObj);
	public event activeDistance ActiveDistance;

	[SerializeField] private Transform testPoint;
	[SerializeField] private float activeDistanceFromCenter = 1;
	private float distance;
	public bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		if ( testPoint == null ) return;

		Vector3 positionA = new Vector3( transform.position.x, 0, transform.position.z );
		Vector3 positionB = new Vector3( testPoint.position.x, 0, testPoint.position.z );

		distance = Vector3.Distance( positionA, positionB);

		if ( distance >= activeDistanceFromCenter && !isActive)
		{

			ActiveDistance( true, this );
			isActive = true;
			print( "Flip" );

		}
		else if( distance < activeDistanceFromCenter && isActive )
		{

			ActiveDistance( false, this );
			isActive = false;

		}
	}
}
