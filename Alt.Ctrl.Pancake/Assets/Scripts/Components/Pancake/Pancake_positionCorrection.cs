using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

/// Corrects the position of the pancake when in frying pan.
/// Prevents the pancake from leaving the confins of the pan
/// when a larg amount of force has been applied to cause a filp.
/// It basicly makes sure that the force is worked out from within the pan 
/// and that all the pancake joints dont all reach 100% causing the pancake to 
/// become some sort of bird shape.
/// 
/// ** This must update after pancake velocity and befor pancake joints **
[RequireComponent(typeof( Pancake_state ) )]
public class Pancake_positionCorrection : MonoBehaviour, IPanCollider, IPancakeStateChanged
{
	[SerializeField] private Renderer meshRenderer;
	private Transform panColliderObj;

	private bool canUpdatePancakeRadius = true;		// we only need to update the pancake radius when the pancake is in a state of mixture.

	[SerializeField]
	private float panRadius = 1;
	[Tooltip("The pan radius offset when the pancake is mixture to prevent the pancake coming up the edges.")]
	[SerializeField] private MinMax mixturePanRadiusOffset = new MinMax(0, 0.75f );
	private float pancakeRadius;
	[SerializeField] private float maxPancakeRadius;

	// this is the max distance from the center of the pancake to the center of the pan collider obj.
	// its is the diferents between the pancake and pan radius.
	private float maxDistanceFromCenter;
	[SerializeField] private float correctionOffset = 0.01f;
	[SerializeField] private float yOffset = 0.05f;

    void Start()
    {
		
		// register onto the state chagned callback on pancake_state.
		// so canUpdatePancakeRadius only gets updated when the state chages.
		GetComponent<Pancake_state>().OnStateChanged += OnPancakeStateChanged;

	}

	void Update()
    {

		if ( panColliderObj == null ) return;

		UpdatePancakeRadius();

		// correct the position of the pancake once we have gone beond maxDistance.
		// ignoring the Y axis.

		Vector3 position = transform.position;
		Vector3 panPosition = panColliderObj.position;
		position.y = panPosition.y = 0;

		float panMixOffset = 0;

		if ( canUpdatePancakeRadius )
			panMixOffset = mixturePanRadiusOffset.GetValue( pancakeRadius / maxPancakeRadius );

		float distance = Vector3.Distance( position, panPosition );

		if ( distance <= maxDistanceFromCenter - panMixOffset ) return;

		// Ooo crap how do i correct the position around a circel?? hmmm.

		// find the angle between the pancake and pan along the forwards axis.
		// so we can rotate the position around the y maintaing the max distance.

		float angle = Mathf.Atan2( panColliderObj.position.x - transform.position.x, transform.position.z - panColliderObj.position.z );

		float sin = Mathf.Sin( angle );
		float cos = Mathf.Cos( angle );

		// get the max position in front of us then rotate it to the required angle
		Vector3 maxPosition = new Vector3( 0, 0, (maxDistanceFromCenter - panMixOffset) + correctionOffset );   
		Vector3 newPosition = Vector3.zero;

		newPosition.x = maxPosition.x * cos - maxPosition.z * sin;
		newPosition.z = maxPosition.z * cos + maxPosition.x * sin;

		// Add the pans world position to the new position to get the final position of the pancake.
		newPosition += panColliderObj.position;
		newPosition.y -= yOffset;

		transform.position = newPosition;
		
    }

	void UpdatePancakeRadius()
	{

		// we only need to update the size of the pancake while it is in a mixture state.
		if ( !canUpdatePancakeRadius ) return;

		Vector3 extents = meshRenderer.bounds.extents;                  // half the size of the bounds, so the distance from this center of the bounds ??

		// For now i thihnk it will be fine if we just work out the avg of the X & Z axis of the bounds for the radius of the pancake
		pancakeRadius = ( extents.x + extents.z ) / 2f;

		// the pancakes radius CAN NOT extend beond the radius of the pan so that must mean that our maxDistanceFromCenter is the differents of the two.
		// ^^ infact the maxRadius of the pancake is less then the radius of the pan. (or atleast it should be...)
		maxDistanceFromCenter = panRadius - pancakeRadius;

	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;
	}

	public void OnPancakeStateChanged( PancakeState state )
	{
		// only update radius when in a state of mixture.
		canUpdatePancakeRadius = state == PancakeState.Mixture;
	}
}
