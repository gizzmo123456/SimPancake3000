using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    public Vector3 force = new Vector3( 0, 10, 0 );
    public bool addForce;
	public bool continuesForce = false;

	public bool SM = false;

    // Update is called once per frame
    void Update()
    {
        
        if(addForce)
        {
			//GetComponent<PancakePhysicsBall>().AddForce(force);
			if ( !SM )
				GetComponent<Rigidbody>().AddForce( force );
			else
				SendMessage( "AddForceToPoint", force );

            addForce = continuesForce;
        }

    }
}

