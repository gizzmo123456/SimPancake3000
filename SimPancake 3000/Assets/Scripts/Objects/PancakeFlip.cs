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

	private Vector3 velocity = Vector3.zero;

	[SerializeField] private float z_transferRate = 2;
	[SerializeField] private float z_counter = 2;
	private bool zReturn = false;

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

		if ( /*fryingPan == null ||*/ velocity == Vector3.zero )
		{
			//transform.eulerAngles = new Vector3( 0, 90, 0 );
			return;
		}
		float rbVel_y = rigid.velocity.y;

		if ( velocity.y < 0 )
			velocity.y = rbVel_y;

		rigid.velocity = new Vector3(0, velocity.y, velocity.z * 0.2f);
		transform.LookAt( transform.position + velocity );

		if ( velocity.z > 0 )
			velocity.y += z_transferRate * Time.deltaTime;

		/*
		else
			velocity.y -= z_transferRate * Time.deltaTime;
		*/
		if(!zReturn)
			velocity.z -= z_transferRate * Time.deltaTime;
		else
			velocity.z += z_counter * Time.deltaTime;

		velocity.y += Physics.gravity.y * Time.deltaTime;

		if ( velocity.z < -50 && !zReturn )
			zReturn = true;
		else if ( zReturn & velocity.z > 0 )
			velocity = Vector3.zero;


		velocity.x = 0;
    }

	public void AddPancakeForce(Vector3 force)
	{
		velocity += force;

	}

}
