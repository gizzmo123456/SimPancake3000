using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manipulate the Z rotation of pancake joints.
/// </summary>
[RequireComponent(typeof(Pancake_jointDistance))]
public class Pancake_joint : MonoBehaviour, IPanCollider
{

	private Pancake_jointDistance jointDistance;
	// tracking object.
	private Transform panColliderObj;

	// collider set up.
	private AnimationCurve colliderCurve;			// this should be in the range of X: 0,1; Y: 0,1;
	/*private*/public float maxDistanceFromCenter;            // X axis of curve
	[SerializeField] private bool scaleJointPosition = false;

	// joint constains
	/*private*/ public float maxCurveRotation = 90f;            // y axis of curve.
	/*private*/ public float maxPositionOffset = 0.05f;
	// ...

	private Vector3 startLocalPosition;

	// DEBUGING
	private float TEMP_DIST;
	private float TEMP_CV;

	private void Start()
	{
		startLocalPosition = transform.localPosition;
	}

	private void Update()
    {
		if ( panColliderObj == null ) return;

		// rotate the joint based on the distance from the center of the panColliderObj.
		// ignoreing the Y axis.

		float distanceFromCenter = TEMP_DIST = scaleJointPosition ? jointDistance.GetDistanceScaled() : jointDistance.GetDistance();// Vector3.Distance( position, panPosition );
		float distancePercent = distanceFromCenter / maxDistanceFromCenter;

		// Get the rotation from the colliderCurve.
		Vector3 rotation = transform.localEulerAngles;
		float curveValue = TEMP_CV = colliderCurve.Evaluate( distancePercent );

		rotation.z = curveValue * maxCurveRotation;

		transform.localEulerAngles = rotation;

		// update the position offset.
		// local
		Vector3 lPos = startLocalPosition;
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
	}

	public void SetupColliderData( AnimationCurve collCurve, float maxDistance )
	{
		jointDistance = GetComponent<Pancake_jointDistance>();
		colliderCurve = collCurve;
		maxDistanceFromCenter = maxDistance;
	}

	public void SetupJointData( float curveRotation, float positionOffset)
	{
		maxCurveRotation = curveRotation;
		maxPositionOffset = positionOffset;
	}
}
