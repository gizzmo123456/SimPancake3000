using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manipulate the Z rotation of pancake joints.
/// </summary>
public class Pancake_joint : MonoBehaviour, IPanCollider
{

	// TODO: ...


	// tracking object.
	private Transform panColliderObj;

	// collider set up.
	private AnimationCurve colliderCurve;			// this should be in the range of X: 0,1; Y: 0,1;
	private float maxDistanceFromCenter;            // X axis of curve

	// joint constains
	private float maxCurveRotation = 90f;            // y axis of curve.
	private float maxPositionOffset = 0.05f;
	// ...

	private Vector3 TEMP_pos;
	private float TEMP_DIST;

    private void Update()
    {
		if ( panColliderObj == null ) return;

		// rotate the joint based on the distance from the cneter of the panColliderObj.
		// ignoreing the Y axis.

		Vector3 position = transform.position;
		Vector3 panPosition = panColliderObj.transform.position;

		position.y = panPosition.y = 0;

		float distanceFromCenter = TEMP_DIST = Vector3.Distance( position, panPosition );
		float distancePercent = distanceFromCenter / maxDistanceFromCenter;

		// Get the rotation from the colliderCurve.
		Vector3 rotation = transform.eulerAngles;
		float curveValue = colliderCurve.Evaluate( distancePercent );

		rotation.z = curveValue * maxCurveRotation;

		transform.eulerAngles = rotation;

		// update the position offset.
		// local
		Vector3 lPos = TEMP_pos;
		lPos.y +=  curveValue * maxPositionOffset ;
	
		transform.localPosition = lPos;

		// world 
		Vector3 wPos = Vector3.zero;
		wPos.y = curveValue * maxPositionOffset;

		transform.position += wPos;
		
	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;
		TEMP_pos = transform.localPosition;
	}

	public void SetupColliderData( AnimationCurve collCurve, float maxDistance )
	{
		colliderCurve = collCurve;
		maxDistanceFromCenter = maxDistance;
	}

	public void SetupJointData( float curveRotation, float positionOffset)
	{
		// ...
		maxCurveRotation = curveRotation;
		maxPositionOffset = positionOffset;
	}
}
