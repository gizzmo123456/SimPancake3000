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


    [Header("Off Hob Distacne.")]

    [SerializeField]
    private float pan_OffHob_YPositionOffset = 10f;
    [SerializeField]
    private float pan_OffHob_minInputValue = 230;
    [SerializeField]
    private float pan_OffHob_maxInputValue = 1023f;

    // Start is called before the first frame update
    void Start()
    {
        startYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        //TODO: check that the panID is in range of pan count once we have a Static Game Class
        if (panID < 0 || panID >= 3) //i know there 3 but still.
        {
            Debug.LogError("Pan Id Out of range (obj name: "+name+")");
            return;
        }

        InputValues inputs = InputHandler.GetInputs();

        Vector3 position = transform.position;
		Vector3 rotation = transform.eulerAngles;

        // Update the Y position of the pan when it has moved on the hob
        position.y = startYPosition + (pan_OffHob_YPositionOffset * (1f - ((inputs.panDistances[panID] - pan_OffHob_minInputValue) / (pan_OffHob_maxInputValue - pan_OffHob_minInputValue))));

		// get the current pan rotation from inputs 
		rotation.x = -inputs.pans_x[ panID ];
		rotation.y = -inputs.pans_y[ panID ];

		// make shore the pancake is awake if the inputs have changed since the last frame :)
		if ( currentPancake != null && ( rotation.x != last_x || rotation.y != last_y ) )
            currentPancake.WakeUp();

        transform.eulerAngles = rotation;
        transform.position = position;

        last_x = x;
        last_y = y;

    }

    public void RegisterPancake(Pancake pancakeToReg)
    {
        currentPancake = pancakeToReg;
    }

    public void UnregisterPancake( Pancake pancakeToUnreg )
    {
        currentPancake = null;
    }

}
