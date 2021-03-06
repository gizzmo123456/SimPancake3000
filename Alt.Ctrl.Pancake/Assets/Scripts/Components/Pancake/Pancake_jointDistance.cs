﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers the flip once a distance is reached.
/// the distance is unaffected by scale
/// </summary>
public class Pancake_jointDistance : MonoBehaviour, IPanCollider
{

	private float maxDistanceFromCenter;
	[SerializeField] private Pancake_panCollision panCollision;
	private Transform panColliderObj;

	private Transform originTranform; // or top-most parent in prefab.
	private Vector3 localOrigin;

	private void Update()
	{
		
		if ( panColliderObj == null ) return;

		float distance = GetDistance();

		// if we are past the max distance attampt to apply upforce/flip :)
		if ( distance > maxDistanceFromCenter )
		{
			panCollision.TransformToUpforce( -transform.right, distance, transform.position.z );      //the joint have been orrentated so that left is forwards, not ideal but thats just how it is. my maya skills are not the best! :|

			if (Pancake_DEBUG.debug_joints)
				Debug.Log( "Trandform to up, name: " + name + " ## Dist: " + distance + " ## Frw: " + ( -transform.right )+" ## yRot: "+transform.eulerAngles.y, gameObject );
		}
	}

	/// <summary>
	/// Get the distance from the center of the object unaffected by scale.
	/// </summary>
	public float GetDistance()
	{
		// find how far from the center we are
		// we store the local orgin of the joint to topmost parent of the pancake.
		// we need to convert it into world space then into the panObj local space to find the local distance 
		Vector3 originPosition = originTranform.TransformPoint( localOrigin );		// from pancake local to world
		originPosition = panColliderObj.InverseTransformPoint( originPosition );	// from world to panObj local

		originPosition.y = 0;

		return Vector3.Distance( originPosition, Vector3.zero );	// get distance reavent to the panObj
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

		Debug.Log( "SETUP FOR: " + name + " wPos:" + transform.position + " # LP: "+transform.localPosition+" # LO: "+localOrigin.x+", "+localOrigin.y+", "+localOrigin.z, gameObject );

		maxDistanceFromCenter = maxDist;
		panCollision = panColl;
	}

}
