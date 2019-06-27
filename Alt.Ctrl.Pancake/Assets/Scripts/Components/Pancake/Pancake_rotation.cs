using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pancake_velocity))]
public class Pancake_rotation : MonoBehaviour, IPanCollider
{
	private Transform panColliderObj;
	private Pancake_velocity velocity;

	private Transform rotateObj;

	[SerializeField] private float rotateSpeed = 180f; // per second

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

		if ( velocity.GetVelocity().y <= 0 ) return;

		// move the pancake into the rot object and rotate around the zAxis,
		// and remove pancake from rotate object.
		// this alows us to rotate the pancane in the same direction as the force when leaving the pan :)

		Vector3 currentVelocity = velocity.GetVelocity();
		float flipSpeed = ( currentVelocity.y / ( ( Mathf.Abs( currentVelocity.x ) + Mathf.Abs( currentVelocity.z ) ) / 2f ) ) * rotateSpeed; //per sec;

		Vector3 currentRot = rotateObj.eulerAngles;
		currentRot.z += flipSpeed * Time.deltaTime;

		// make sure the rotateObj is in the same position as the pancake befor makeing the pancake a child
		// so we always rotating from the center...
		rotateObj.position = transform.position;

		transform.parent = rotateObj;
		rotateObj.eulerAngles = currentRot;

		transform.parent = null;
		
		// TODO: level flip off when below flipspeed thresshold??

    }

	public void SetPanCollider( Transform panCollObj )
	{
		panColliderObj = panCollObj;
	}

	public void SetFlipRotation()
	{

		Vector3 velNorm = velocity.Velocity.normalized;
		// flip the x/z axis since the bones are orineted so that x is forwards :|
		// and set y to zero so we dont look up and down :)
		velNorm.x = velocity.Velocity.normalized.z;
		velNorm.y = 0;
		velNorm.z = velocity.Velocity.normalized.x;

		rotateObj.LookAt( rotateObj.position + velNorm );

	}

	private void OnDestroy()
	{
		// destroy rotate object, we dont want a scene full of empty GO's
		Destroy(rotateObj.gameObject);
	}

}
