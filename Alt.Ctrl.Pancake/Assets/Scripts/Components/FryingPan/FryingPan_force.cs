using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FryingPan_pancake))]
public class FryingPan_force : BasePanGroup_multipleInput
{
	protected override int RequiredInputs { get { return 2; } }
	private FryingPan_pancake pancake;

	[Header("Force")]
	[SerializeField] private Vector3 slideVelocity = new Vector3( 1, 0, 1 );
	[SerializeField] private Vector3 flipVelocity = new Vector3( 2, 0, 6 );

	[SerializeField, Range( 0, 1 )]
	private int inputId_xRotation = 0;

	[SerializeField, Range( 0, 1 )]
	private int inputId_zRotation = 1;

	private Vector3 currentVelocity = Vector3.zero;
	[SerializeField] private float maxDeltaRotation = 20;
	[SerializeField] private float DeltaRotationThresshold = 2;
	private Vector3 rotateDelta = Vector3.zero;
	private Vector3 lastRotation = Vector3.zero;

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
		if ( pancake.PancakeCount == 0 || !pancake.IsMixturePancake() ) return;

		// So, i fucked up...
		// To work out the force to give the pancake lift off we new to work it out the force by its delta
		// so we know what direct the pan is being rotated in thus knowing the direction to apply force.
		// The centered input value should be used to slide the pancake around the pan as is being rotates.
		// ... Well kinda.

		Vector3 curVel = Vector3.zero;

		// We only need to use the delta force for forards, left/right. Theres no point applying backwards force
		// for fliping the pancake, it just can't happen. Imagin that in the real world, rotating the pan down realy
		// fast and getting a face full of scorching pancake.

		if ( rotateDelta.x > DeltaRotationThresshold )												// forwards
			curVel.x = (rotateDelta.x / maxDeltaRotation) * flipVelocity.x;

		if (rotateDelta.z > DeltaRotationThresshold || rotateDelta.z < -DeltaRotationThresshold)	// left/right
			curVel.z = (-rotateDelta.z / maxDeltaRotation) * flipVelocity.z;

		// add some slide force (velocity) to them pancakes.
		// Normalized the inputs values to the center of the input, so when the pan is flat the value is 0,0,0 :)
//		curVel.x += ( ( inputValues[ inputId_xRotation ].ClampedPrecent - 0.5f ) / 0.5f ) * slideVelocity.y;
//		curVel.z += ( ( inputValues[ inputId_zRotation ].ClampedPrecent - 0.5f ) / 0.5f ) * slideVelocity.z;

		currentVelocity = curVel;

		// Semnd velocity to all pancakes in the frying pan.
		foreach ( Pancake_state pancake in pancake.GetPancakes() )
		{       // TODO: Add Pancake class with a ref to all of its outher classes, i dont have to keep using GetComponet :)
			pancake.GetComponent<Pancake_velocity>().AddVelocity( currentVelocity );
		}

	}

}
