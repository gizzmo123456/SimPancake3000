using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobNob : MonoBehaviour
{
    [SerializeField]
    private int hobID = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        InputValues inputs = InputHandler.GetInputs();

        //TODO: check that the panID is in range of pan count once we have a Static Game Class
        if (hobID < 0 || hobID >= 3) //i know there 3 but still.
        {
            Debug.LogError("Pan Id Out of range (obj name: " + name + ")");
            return;
        }

        Vector3 currentRotation = transform.eulerAngles;

        currentRotation.z = -270f * (inputs.hobs[hobID] / 1023f);
        transform.eulerAngles = currentRotation;
        print(inputs.hobs[hobID]);

    }
}
