using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobFire : MonoBehaviour
{
    [SerializeField]
    private int hobID = 0;
    [SerializeField]
    private Vector3 minScale = new Vector3(0.4f, 0.4f, 0.4f);
    [SerializeField]
    private Vector3 scaleDif = new Vector3(0.5f, 0.5f, 0.5f);

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

        Vector3 newScale = minScale + (scaleDif * (inputs.hobs[hobID] / 1023f));

        transform.localScale = newScale;

    }
}
