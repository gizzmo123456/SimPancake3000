﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up pancake joints
/// </summary>
public class Pancake_jointSetup : MonoBehaviour, IPanCollider
{

	private Transform panColliderObj;
	private Pancake_joint[] joints;

	[SerializeField] private AnimationCurve colliderCurve;
	[SerializeField] private float maxDistanceFromCenter;

	[SerializeField] private float maxJointRotation;
	[SerializeField] private float maxPositionOffset;
	[Range(0f, 1f), Tooltip("Only affects joint rotation")]
	[SerializeField] private float[] jointDepthWeight;

	[SerializeField] private float transformForceDistance;


	private void Awake()
	{

		joints = FindChildrenWithJoints( transform ).ToArray();

	}

	/// <summary>
	/// find all the children with joint in the.
	/// </summary>
	/// <param name="parent">transform to search</param>
	/// <param name="nestedId">the depth of child in hierarchy from objects root</param>
	/// <returns></returns>
	List<Pancake_joint> FindChildrenWithJoints(Transform parent, int nestedId = 0)		// TODO: i havent set the depth anywhere yet :|
	{
		List<Pancake_joint> pancakeJoints = new List<Pancake_joint>();

		for (int i = 0; i < parent.childCount; i++ )
		{
			Transform child = parent.GetChild( i );

			if ( child.childCount > 0 )
				pancakeJoints.AddRange( FindChildrenWithJoints( child, nestedId + 1) );

			if ( !child.name.Contains( "joint" ) ) continue;

			float jointWeight = nestedId < jointDepthWeight.Length ? jointDepthWeight[ nestedId ] : 1f;

			Pancake_joint childJoint = child.gameObject.AddComponent<Pancake_joint>();
			childJoint.SetupColliderData(colliderCurve, maxDistanceFromCenter);
			childJoint.SetupJointData(maxJointRotation * jointWeight, maxPositionOffset);

			Pancake_jointDistance distanceJoint = child.gameObject.AddComponent<Pancake_jointDistance>();
			distanceJoint.Setup( transformForceDistance, GetComponent<Pancake_panCollision>() );

			pancakeJoints.Add( childJoint );
			
		}

		return pancakeJoints;

	}

	public void SetPanCollider(Transform gameObj)
	{

		panColliderObj = gameObj;

		//NOTE: it might be better if iv stored them all as IPanCollider.
		//Update all the children joints
		foreach ( Pancake_joint joint in joints )
		{
			joint.SetPanCollider( gameObj );
			joint.GetComponent<Pancake_jointDistance>().SetPanCollider( gameObj );
		}
	}


}
