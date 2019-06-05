using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// it might be an idear to rename this class.
// since it will now deal with all pancake forces :)

/*	Read Note Above^^
 *  Handles the pancakes forces (includeing flip)
 *  
 */
 [RequireComponent(typeof(Rigidbody))]
public class PancakeFlip : MonoBehaviour
{

	private FryingPan fryingPan;
	private Rigidbody rigid;

	public bool debug = false;

	private Vector3 forceToAdd = Vector3.zero;
	private Vector3 velocity = Vector3.zero;

	private float fryingPan_lastZRotation;

	private void Awake()
	{

		rigid = GetComponent<Rigidbody>();

	}

	public void SetFryingPan( FryingPan pan )
	{
		fryingPan = pan;
	}

	void FixedUpdate()
    {

		if ( fryingPan == null ) return;

		// project the current velocity to corrospond to the frying pans new rotation since the last frame.
		// befor adding any new force :)
		float fryingPan_deltaZRotation = fryingPan.transform.eulerAngles.z - fryingPan_lastZRotation;

		float sin = Mathf.Sin( fryingPan_deltaZRotation );
		float cos = Mathf.Cos( fryingPan_deltaZRotation );

		float y = velocity.y;
		float z = velocity.z;

		velocity.y = y * cos + z * sin;
		velocity.z = z * cos - y * sin;

		velocity += forceToAdd;

		rigid.velocity = velocity;

		//velocity -= Physics.gravity * Time.deltaTime;

		forceToAdd = Vector3.zero;
		fryingPan_lastZRotation = fryingPan.transform.eulerAngles.z;

    }

	public void AddPancakeForce(Vector3 force)
	{

		forceToAdd = force;

	}

}
