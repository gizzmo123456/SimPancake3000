using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    public Vector3 force = new Vector3( 0, 10, 0 );
    public bool addForce;

    // Update is called once per frame
    void Update()
    {
        
        if(addForce)
        {
            GetComponent<PancakePhysicsBall>().AddForce(force);
            addForce = false;
        }

    }
}
