using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake : MonoBehaviour
{

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //never sleep :)
        if(rb.IsSleeping())
        {
            print("Stop sleeping on the job!");
            rb.WakeUp();
        }

    }
}
