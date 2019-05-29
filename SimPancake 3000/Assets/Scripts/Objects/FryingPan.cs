using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan : MonoBehaviour
{
    [SerializeField] private int panID = 0;

	private float startYPosition;

	private float last_x_rotation, last_z_rotation;
	private float last_z_deltaRotation;
	private float last_y_position;

	[Header( "Pancake" )]
	private Pancake currentPancake;				// <-- this will have to be a list to support mutiple pancakes :)

	private float acumalatedPancakeForce = 0;	// <-- it might be better if this was in the pancake itself. :)
	private int acumPancakeForce_frameCount = 0;

	[SerializeField] private Pancake pancake_prefab;

	[SerializeField]
	private float forceMuti = 5f;

	[ Header("Off Hob Distacne.")]

    [SerializeField] private float pan_OffHob_YPositionOffset = 10f;
    [SerializeField] private float pan_OffHob_minInputValue = 230;
    [SerializeField] private float pan_OffHob_maxInputValue = 1023f;

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
		rotation.z = -inputs.pans_y[ panID ];		//<-- Hmm, this is a lil confusing. Y on the Gyro is z in unity. TODO: do somthink to clear this up :), ie. rename the array.

		// make shore the pancake is awake if the inputs have changed since the last frame :)
		if ( currentPancake != null && ( rotation.x != last_x_rotation || rotation.y != last_z_rotation || position.y != last_y_position) )
            currentPancake.WakeUp();

        transform.eulerAngles = rotation;
        transform.position = position;

		ApplyForceToPancakes( rotation.z - last_z_rotation );

		// record the last rotation and y position so we know whether or not to wake up any pancake that are in the pan 
		last_x_rotation = rotation.x;
        last_z_rotation = rotation.z;

		last_y_position = position.y;

    }

	private void ApplyForceToPancakes( float zRotationDelta )
	{

		if ( currentPancake && zRotationDelta > 0 && zRotationDelta >= last_z_deltaRotation)
		{
			acumalatedPancakeForce += zRotationDelta;
			acumPancakeForce_frameCount++;
		}
		else if ( acumPancakeForce_frameCount > 0 )
		{
			if ( currentPancake )
			{
				currentPancake.AddForce( (acumalatedPancakeForce / (float)acumPancakeForce_frameCount ) * forceMuti );
			}

			acumalatedPancakeForce = 0;
			acumPancakeForce_frameCount = 0;
			
		}

		last_z_deltaRotation = zRotationDelta;

	}

    public void RegisterPancake(Pancake pancakeToReg)
    {
		currentPancake = pancakeToReg;
		currentPancake.transform.parent = transform;
    }

    public void UnregisterPancake( Pancake pancakeToUnreg )
    {
		currentPancake.transform.parent = null;
		currentPancake = null;
    }

	// called from the batter when it collides with the fryingPan.
	public void BatterCollision(Vector3 collisionPosition, float batterQt)
	{
		Quaternion quaternion = new Quaternion();
		quaternion.eulerAngles = new Vector3( 0, 90, 0 );

		// create a new pancake if we are not already reciving batter :)
		if ( !currentPancake )
		{
			currentPancake = Instantiate( pancake_prefab, collisionPosition, quaternion );
			currentPancake.SetStartPosition( collisionPosition );
		}
		else if( currentPancake && currentPancake.GetCurrentState() != PancakeState.Raw )	// consume any current pancakes in the pan that are not raw.
		{
			Pancake oldPancake = currentPancake;
			currentPancake = Instantiate( pancake_prefab, collisionPosition, quaternion );
			currentPancake.SetStartPosition( collisionPosition );
			oldPancake.transform.parent = currentPancake.transform;
		}

		currentPancake.transform.parent = transform;
		currentPancake.AddBatter( batterQt, 0.25f );	// 0.25f is the spwan intervals of batter from jug. 

	}


}
