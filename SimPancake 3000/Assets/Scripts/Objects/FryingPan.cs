using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

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
    [SerializeField] private MinMax pan_OffHob_minMaxInputValue = new MinMax(230f, 1023f);

	[Header( "Pan Temperture" )]
	[SerializeField] private float currentTemperture = 0;   // max temp is defined by curve.
	[SerializeField] private AnimationCurve tempertureCurve = new AnimationCurve();
	[SerializeField] private float maxTempture = 200;
	[SerializeField] private float coolDownRate = 1f;

	public bool debug = false;

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
		pan_OffHob_minMaxInputValue.current = inputs.panDistances[ panID ];

		Vector3 position = transform.position;
		Vector3 rotation = transform.eulerAngles;

		// Update the Y position of the pan when it has moved on the hob
		position.y = startYPosition + (pan_OffHob_YPositionOffset * (1f - pan_OffHob_minMaxInputValue.ClampedPrecent));

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

		SetPancakeTemperture();

    }

	private void ApplyForceToPancakes( float zRotationDelta )
	{

		if ( currentPancake && zRotationDelta > 0 && zRotationDelta >= last_z_deltaRotation)
		{
			acumalatedPancakeForce += zRotationDelta;
			acumPancakeForce_frameCount++;

			currentPancake.SendMessage( "AddPancakeForce", (zRotationDelta * forceMuti) * transform.right );	// TODO: i think this is the only thing we usin in this function. so remove all the bs
		}
		else if ( acumPancakeForce_frameCount > 0 )
		{
			if ( currentPancake )
			{
			//	currentPancake.AddForce( (acumalatedPancakeForce / (float)acumPancakeForce_frameCount ) * forceMuti );
			}

			acumalatedPancakeForce = 0;
			acumPancakeForce_frameCount = 0;
			
		}

		last_z_deltaRotation = zRotationDelta;

	}

    public void RegisterPancake(Pancake pancakeToReg)
    {
		// can not reg a raw pancake. this is delta with in batterCollision.
		if ( pancakeToReg.GetCurrentState() == PancakeState.Mixture )
			return;

		currentPancake = pancakeToReg;
		currentPancake.transform.parent = transform;
		currentPancake.SetCurrentPan( this );

    }

    public void UnregisterPancake( Pancake pancakeToUnreg )
    {
		//can not remove a pan if its raw or not there :)
		if ( !currentPancake || currentPancake && currentPancake.GetCurrentState() == PancakeState.Mixture )
			return;

		//Set the temperature back to 0, sinc it is no longer in the pan
		currentPancake.SetTemperature(0);
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
			currentPancake.transform.parent = transform;
			currentPancake.SetStartPosition( collisionPosition );
			currentPancake.SetCurrentPan( this );
		}
		else if( currentPancake && currentPancake.GetCurrentState() != PancakeState.Mixture )	// consume any current pancakes in the pan that are not raw.
		{
			Pancake oldPancake = currentPancake;
			currentPancake = Instantiate( pancake_prefab, collisionPosition, quaternion );
			currentPancake.transform.parent = transform;
			currentPancake.SetStartPosition( collisionPosition );
			oldPancake.transform.parent = currentPancake.transform;
			currentPancake.SetCurrentPan( this );
		}

		currentPancake.AddBatter( batterQt, 0.25f );	// 0.25f is the spwan intervals of batter from jug. 

	}

	public void AddTempture(float tempPerSec)
	{

		float addAmountMulti = 0;
		//center the value so we get it in the range of -1 to 1 so we can cool the pan down when its to far away.
		// < 0 is cool down >= 0 is heat up.
		float panDistanceMulti = ( ( -0.5f + pan_OffHob_minMaxInputValue.ClampedPrecent ) / 0.5f ) ;
		float tempToAdd = 0;

		if ( panDistanceMulti < 0 )
		{
			addAmountMulti = tempertureCurve.Evaluate( 1f - Mathf.Clamp01(currentTemperture / maxTempture) );
			tempPerSec = coolDownRate;
		}
		else
		{
			addAmountMulti = tempertureCurve.Evaluate( Mathf.Clamp01( currentTemperture / maxTempture ) );
		//	panDistanceMulti = 1 - panDistanceMulti;
		}

		tempToAdd = ( tempPerSec * Time.deltaTime ) * panDistanceMulti * addAmountMulti;

		if ( debug )
			print( panDistanceMulti +" ## "+ tempToAdd +" ### "+ addAmountMulti +" ### "+ Mathf.Clamp01( currentTemperture / maxTempture ) );

		currentTemperture += tempToAdd;

	}

	/// <summary>
	/// Get an array of all the current pancakes avable in the pan.
	/// </summary>
	// TODO: Update to array once i have added muti pancake support.
	public Pancake GetCurrentPancakes()
	{
		return currentPancake;
	}

	private void SetPancakeTemperture()
	{

		if ( currentPancake == null ) return;

		currentPancake.SetTemperature( currentTemperture );

	}
}
