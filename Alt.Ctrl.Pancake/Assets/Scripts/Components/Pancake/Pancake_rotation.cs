using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pancake_velocity))]
public class Pancake_rotation : MonoBehaviour, IPanCollider
{
	private Transform panColliderObj;
	private Pancake_velocity velocity;

	private Transform rotateObj;

	private void Awake()
	{
		rotateObj = new GameObject().transform;
	}

	// Start is called before the first frame update
	void Start()
    {

		velocity = GetComponent<Pancake_velocity>();

    }

    // Update is called once per frame
    void Update()
    {

		if ( panColliderObj != null ) return;   // dont rotate if we are in the pan.

		if ( velocity.GetVelocity().y < 0 ) return;

		// move the pancake into the rot object and rotate around the zAxis,
		// and remove pancake from rotate object.
		// this alows us to rotate the pancane in the same direction as the force :)

		Vector3 currentVelocity = velocity.GetVelocity();
		float flipSpeed = currentVelocity.y / ( currentVelocity.x + currentVelocity.y ); //per sec;

		Vector3 currentRot = rotateObj.eulerAngles;
		currentRot.z += flipSpeed * Time.deltaTime;

		// make shore the rotateObj is in the same position as the pancake befor makeing the pancake a child
		// so we always rotating from the center...
		rotateObj.position = transform.position;

		transform.parent = rotateObj;
		rotateObj.eulerAngles = currentRot;

		transform.parent = null;
		
    }

	public void SetPanCollider( Transform panCollObj )
	{
		panColliderObj = panCollObj;
	}

	public void SetFlipRotation(float yRot)
	{

		Vector3 rot = Vector3.zero;
		rot.y = yRot;

		rotateObj.eulerAngles = rot;

	}

}
