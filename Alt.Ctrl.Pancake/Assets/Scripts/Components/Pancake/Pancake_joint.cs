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
	private Quaternion startLocalRotation;

	[SerializeField] private float flattenSpeed_rotation = 1f;
	[SerializeField] private float flattenSpeed_position = 2f;

	// DEBUGING
	private float TEMP_DIST;
	private float TEMP_CV;

	private void Start()
	{
		startLocalPosition = transform.localPosition;
		startLocalRotation = transform.localRotation;
	}

	private void Update()
    {
		if ( panColliderObj == null )
			FlatenJoint();
		else
			DynamicJointRotation();

	}

	public void DynamicJointRotation()
	{

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
		lPos.y += curveValue * maxPositionOffset;

		transform.localPosition = lPos;

		// world 
		Vector3 wPos = Vector3.zero;
		wPos.y = curveValue * maxPositionOffset;

		transform.position += wPos;

	}

	public void FlatenJoint()
	{

		// if there is any rotation on the joint or we are out of local position 
		// lerp it back to the default position / rotation

		// work out the rotation step by the z axis, since this is the only axis that really rotates.
		// and it must be absolute, as if its negative it will rotate to the opersite angle.
		float rotationStep = Mathf.Abs( transform.localEulerAngles.z - startLocalRotation.eulerAngles.z ) * flattenSpeed_rotation * Time.deltaTime;
		Vector3 localPositionDif = transform.localPosition - startLocalPosition;

		if ( rotationStep > 0f || localPositionDif != Vector3.zero )
		{

			// return to the default position
			// this will never be perfect, but cloese enought :)

			Vector3 position = transform.localPosition;
			position -= localPositionDif * flattenSpeed_position * Time.deltaTime;

			transform.localPosition = position;
			
			// rotate back to the start rotation via the shortest path.
			transform.localRotation = Quaternion.RotateTowards( transform.localRotation, startLocalRotation, rotationStep );

/*
			if ( name.ToLower() == "joint51" )
				Debug.LogWarning( "SP: "+startLocalRotation+" ## rDiff: "+localRotationDif+" ## rot: " + rotation, gameObject );
*/
		}

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

	public void SetupJointData( float curveRotation, float positionOffset, float flattenSpeed_rot, float flattenSpeed_pos)
	{
		maxCurveRotation = curveRotation;
		maxPositionOffset = positionOffset;
		flattenSpeed_rotation = flattenSpeed_rot;
		flattenSpeed_position = flattenSpeed_pos;
	}
}
