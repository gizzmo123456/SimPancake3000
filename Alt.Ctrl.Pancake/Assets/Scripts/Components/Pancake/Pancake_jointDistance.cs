using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_jointDistance : MonoBehaviour, IPanCollider
{

	private float maxDistanceFromCenter;
	[SerializeField] private Pancake_panCollision panCollision;
	private Transform panColliderObj;

	private void Update()
	{

		if ( panColliderObj == null ) return;

		// find how far from the center we are
		Vector3 position = transform.position;
		Vector3 panPosition = panColliderObj.transform.position;

		position.y = panPosition.y = 0;

		float distance = Vector3.Distance( position, panPosition );

		// if we are past the max distance attampt to apply upforce/flip :)
		if ( distance > maxDistanceFromCenter )
			panCollision.TransformToUpforce(transform.forward);

	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;
	}

	public void Setup( float maxDist )
	{
		maxDistanceFromCenter = maxDist;
	}
}
