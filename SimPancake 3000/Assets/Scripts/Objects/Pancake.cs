using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pancake : MonoBehaviour
{

    private Rigidbody rb;
    private FryingPan currentPan;   //null if its not in a pan

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        WakeUp();
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
		Debug.LogError("Adding Force: "+force);
		rb.AddForce( new Vector3( 0, force, 0 ) );
	}

}
