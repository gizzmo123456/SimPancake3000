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

	private Rigidbody rigid;

	public bool debug = false;

	private Vector3 velocity = Vector3.zero;

	private void Awake()
	{

		rigid = GetComponent<Rigidbody>();

	}

	void FixedUpdate()
    {

    }

	public void AddPancakeForce(Vector3 force)
	{

		velocity += force;

	}

}
