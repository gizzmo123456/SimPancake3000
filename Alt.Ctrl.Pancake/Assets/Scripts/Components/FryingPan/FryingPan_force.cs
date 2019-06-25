using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FryingPan_pancake))]
public class FryingPan_force : BasePanGroup_multipleInput
{

	protected override int RequiredInputs { get { return 2; } }
	private FryingPan_pancake pancake;
	[SerializeField] private Vector3 maxVelocity = new Vector3( 1, 0, 1 );

	[SerializeField, Range( 0, 1 )]
	private int inputId_xRotation = 0;

	[SerializeField, Range( 0, 1 )]
	private int inputId_zRotation = 1;

	private Vector3 currentVelocity = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();

		pancake = GetComponent<FryingPan_pancake>();

	}

	protected override void Update()
    {

		base.Update();

		// check that theres is at least one pancake in the pan to send the pans force to.
		// making sure that it is not mixture.
		if ( pancake.PancakeCount == 0 || !pancake.IsMixturePancake() ) return;

		// add some force (velocity) to them pancakes.
		// Normalized the inputs values to the center of the input, so when the pan is flat the value is 0,0,0 :)
		currentVelocity.x = ( ( inputValues[ inputId_xRotation ].ClampedPrecent - 0.5f ) / 0.5f ) * maxVelocity.y;
		currentVelocity.z = ( ( inputValues[ inputId_zRotation ].ClampedPrecent - 0.5f ) / 0.5f ) * maxVelocity.z;
		
		// Semnd velocity to all pancakes in the frying pan.
		foreach ( Pancake_state pancake in pancake.GetPancakes() )
		{       // TODO: Add Pancake class with a ref to all of its outher classes, i dont have to keep using GetComponet :)
			pancake.GetComponent<Pancake_velocity>().AddVelocity( currentVelocity );
		}

    }
}
