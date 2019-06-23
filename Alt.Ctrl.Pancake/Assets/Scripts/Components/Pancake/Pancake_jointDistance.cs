using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_jointDistance : MonoBehaviour, IPanCollider
{

	private float maxDistanceFromCenter;
	[SerializeField] private Pancake_panCollision panCollision;
	private Transform panColliderObj;

	private Transform originTranform; // or top-most parent in prefab.
	private Vector3 localOrigin;

	public float TEMPDIST = 0;	//DEBUG...

	private void Update()
	{

		if ( panColliderObj == null ) return;

		// find how far from the center we are
		Vector3 originPosition = originTranform.TransformPoint( localOrigin ); //transform.position;
		Vector3 panPosition = panColliderObj.transform.position;

		originPosition.y = panPosition.y = 0;

		float distance = TEMPDIST = Vector3.Distance( originPosition, panPosition );

		// if we are past the max distance attampt to apply upforce/flip :)
		if ( distance > maxDistanceFromCenter )
		{
			panCollision.TransformToUpforce( -transform.right, distance, name );      //the joint have been orrentated so that left is forwards, not ideal but thats just how it is. my myay skills are not the best! :|
			Debug.Log( "Trandform to up, name: " + name + " ## Dist: " + distance + " ## Frw: " + ( -transform.right ), gameObject );
		}
	}

	public void SetPanCollider( Transform panCollObj)
	{
		panColliderObj = panCollObj;
	}

	public void Setup( Transform origin, float maxDist, Pancake_panCollision panColl )
	{
		// get the position of this joint localy to its top-most parent (in terms of prefab).
		originTranform = origin;

		// store the current scale of the origin so we can get the joints position in the origins local space when the scale is 1,1,1 .
		// then return it back to the original scale. (maybe not the best way to do this but proberly the simperlist)
		Vector3 originScale = origin.localScale;
		origin.localScale = Vector3.one;
		localOrigin = originTranform.InverseTransformPoint( transform.position );
		origin.localScale = originScale;

		Debug.Log( "SETUP FOR: " + name + " wPos:" + transform.position, gameObject );

		maxDistanceFromCenter = maxDist;
		panCollision = panColl;
	}

}
