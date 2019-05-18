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

}
