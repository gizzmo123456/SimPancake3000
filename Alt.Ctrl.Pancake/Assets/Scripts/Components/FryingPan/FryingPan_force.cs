using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof( FryingPan_pancake ) )]
public class FryingPan_force : BasePanGroup_multipleInput
{
	protected override int RequiredInputs { get { return 2; } }
	private FryingPan_pancake pancake;

	[Header( "Force" )]
	[SerializeField] private Vector3 slideVelocity = new Vector3( 1, 0, 1 );
	[SerializeField] private Vector3 flipVelocity = new Vector3( 2, 0, 6 );

	[SerializeField, Range( 0, 1 )]
	private int inputId_xRotation = 0;

	[SerializeField, Range( 0, 1 )]
	private int inputId_zRotation = 1;

	[SerializeField] private float maxDeltaRotation = 20;
	[SerializeField] private float deltaRotationThresshold = 2;
	private Vector3 rotateDelta = Vector3.zero;
	private Vector3 lastRotation = Vector3.zero;

	private Vector3 lastInputPercentage = Vector3.zero;     // range -1 to 1


	protected override void Awake()
	{
		base.Awake();

		pancake = GetComponent<FryingPan_pancake>();

	}

	protected override void Update()
	{

		base.Update();

		// Get the delta rotation of the pan since the last update.
		rotateDelta.x = inputValues[ inputId_xRotation ].current;
		rotateDelta.z = inputValues[ inputId_zRotation ].current;

		rotateDelta -= lastRotation;

		// Hmmm... I think this might update the velocity. ;)
		UpdateVelocity();

		// remember the rotation on the pan, so it can be used next frame :P
		lastRotation.x = inputValues[ inputId_xRotation ].current;
		lastRotation.z = inputValues[ inputId_zRotation ].current;

	}

	private void UpdateVelocity()
	{
		// check that theres is at least one pancake in the pan to send the pans force to.
		// making sure that it is not mixture.
		if ( pancake.PancakeCount == 0 || pancake.IsMixturePancake() ) return;

		// So, i fucked up...
		// To work out the force to give the pancake lift off we new to work it out the force by its delta
		// so we know what direction the pan is being rotated in thus knowing the direction to apply force.
		// The centered (-1, 1) input value should be used to slide the pancake around the pan as its being rotates.
		// ... Well kinda.

		Vector3 curVel = Vector3.zero;
		Vector3 curSlideVel = Vector3.zero;

		// We only need to use the delta force for forards, left/right. Theres no point applying backwards force
		// for fliping the pancake, it just can't happen. Imagin that in the real world, rotating the pan down realy
		// fast and getting a face full of scorching pancake.

		if ( rotateDelta.x > deltaRotationThresshold )                                              // forwards
			curVel.x = ( rotateDelta.x / maxDeltaRotation ) * flipVelocity.x;

		if ( rotateDelta.z > deltaRotationThresshold || rotateDelta.z < -deltaRotationThresshold )  // left/right
			curVel.z = ( -rotateDelta.z / maxDeltaRotation ) * flipVelocity.z;

		// add some slide force (velocity) to them pancakes.

		curSlideVel = GetSlideForce().Multiply(slideVelocity);

		// Send velocity to all pancakes in the frying pan.
		foreach ( Pancake_state pancake in pancake.GetPancakes() )
		{       // TODO: Add Pancake class with a ref to all of its outher classes, i dont have to keep using GetComponet :)
			pancake.GetComponent<Pancake_velocity>().AddVelocity( curVel, Pancake_velocity.VelocityType.Default );
			pancake.GetComponent<Pancake_velocity>().AddVelocity( curSlideVel, Pancake_velocity.VelocityType.Limited );
		}

	}

	private Vector3 GetSlideForce()
	{
		// Get the input values between -1, 1 so when the pan is flat it is zero
		Vector3 inputPercentage = Vector3.zero;
		inputPercentage.x = ( inputValues[ inputId_xRotation ].ClampedPrecent - 0.5f ) / 0.5f; // range -1 to 1
		inputPercentage.z = ( inputValues[ inputId_zRotation ].ClampedPrecent - 0.5f ) / 0.5f; // range -1 to 1

		Vector3 slideForce = Vector3.zero;
		slideForce.x = GetAxisSlideForce( lastInputPercentage.x, inputPercentage.x );
		slideForce.z = GetAxisSlideForce( lastInputPercentage.z, inputPercentage.z );

		lastInputPercentage = inputPercentage;

		return slideForce;
	}

	private float GetAxisSlideForce(float lastInputValue, float inputValue)
	{

		float mod = 1;	// pos/neg modifier

		// make the last input value positve.
		// this means we have to invert the input and mod value as well.
		// so we can put corrent the +/- when the value is returned.
		if(lastInputValue < 0)
		{
			lastInputValue = -lastInputValue;
			inputValue = -inputValue;
			mod = -1;
		}

		// Therefor we only need to check if the input is grater than last input or less then zero
		// since its granteed to be positive.
		if ( inputValue > lastInputValue || inputValue < 0 )
			return (inputValue - lastInputValue) * mod;

		return 0;	

	}
}
