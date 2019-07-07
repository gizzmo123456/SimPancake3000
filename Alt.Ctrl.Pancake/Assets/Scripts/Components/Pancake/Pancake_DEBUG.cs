using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_DEBUG : MonoBehaviour
{

	public bool debugJoints = false;
	public static bool debug_joints = false;

	public GameObject pancake;
	public Transform panCollider;
	public Vector3 velocity;
	public bool setPanCollider = false;
	public bool updatePosition = false;
	public bool addVelocity = false;
	public bool killVelocity = false;
	public bool changePancakeSide = false;

    void Update()
    {

		if ( debugJoints != debug_joints )
			debug_joints = debugJoints;

		if ( pancake == null ) return;

		if( setPanCollider )
		{
			pancake.SendMessage("SetPanCollider", panCollider);
			setPanCollider = false;
		}

		if ( updatePosition )
		{
			Pancake_panCollision panColl = pancake.GetComponent<Pancake_panCollision>();

			if( panColl.GetPanCollider() != null )
				panColl.positionInPan = panColl.GetPanCollider().InverseTransformPoint(transform.position);
		}

		if( addVelocity )
		{
			pancake.SendMessage( "AddVelocity", velocity );
			addVelocity = false;
		}

		if (killVelocity)
		{
			pancake.SendMessage( "SetVelocity", Vector3.zero );
			killVelocity = false;
		}

		if (changePancakeSide)
		{
			pancake.SendMessage("ChangeSideDown");
			changePancakeSide = false;
		}

    }
}
