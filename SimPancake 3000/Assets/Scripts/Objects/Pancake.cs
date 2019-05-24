using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pancake : MonoBehaviour
{

    private Rigidbody rb;
    private FryingPan currentPan;   //null if its not in a pan

	private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
		startPosition = transform.position;
    }

    void Update()
    {
        WakeUp();

		//TEMP RESET!
		if ( transform.position.y < -3 )
		{
			transform.position = startPosition;
			rb.velocity = Vector3.zero;
		}

    }

    public void SetCurrentPan(FryingPan fryingPan)
    {
        currentPan = fryingPan;
    }

    public void WakeUp()
    {
        if (rb.IsSleeping())
        {
            print("Stop sleeping on the job!");
            rb.WakeUp();
        }
    }

	public void AddForce(float force)
	{
		rb.AddForce( new Vector3( 0, force, 0 ) );
		//rb.AddTorque(new Vector3(-1500f * force, 0, 0) );
	}

}
