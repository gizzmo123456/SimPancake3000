using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan : MonoBehaviour
{
    [SerializeField]
    private int panID = 0;
    private int last_x, last_y;

    Pancake currentPancake;

    private float startYPosition;
    [SerializeField]
    private float panPositionOffset = 10f;

    [SerializeField]
    private float minInputValue = 230;
    [SerializeField]
    private float maxInputValue = 1023f;
    // Start is called before the first frame update
    void Start()
    {
        startYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        InputValues inputs = InputHandler.GetInputs();

        //TODO: check that the panID is in range of pan count once we have a Static Game Class
        if (panID < 0 || panID >= 3) //i know there 3 but still.
        {
            Debug.LogError("Pan Id Out of range (obj name: "+name+")");
            return;
        }

        int x = inputs.pans_x[panID];
        int y = inputs.pans_y[panID];

        Vector3 position = transform.position;
        print(inputs.panDistances[panID]);
        position.y = startYPosition + (panPositionOffset * (1f - ((inputs.panDistances[panID] - minInputValue) / (maxInputValue - minInputValue))));

        // make shore the pancake is awake if the inputs have changed since the last frame :)
        if (currentPancake != null && (x != last_x || y != last_y))
            currentPancake.WakeUp();

        transform.eulerAngles = new Vector3(-x, -90, -y);
        transform.position = position;

        last_x = x;
        last_y = y;

    }

    private void OnTriggerEnter(Collider coll)
    {
        
        if(coll.CompareTag("pancake"))
        {
            currentPancake = coll.gameObject.GetComponent<Pancake>();
            currentPancake.SetCurrentPan(this);
        }

    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("pancake"))
        {
            currentPancake.SetCurrentPan(null);
            currentPancake = null;
        }
    }

}
