using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;	

public class FryingPan_rotation : BasePanGroup_multipleInput
{
	protected override int RequiredInputs { get { return 2; } }

	[Header( "FryingPan Rotation" )]

	[SerializeField, Range( 0, 1 )]
	private int inputId_xRotation = 0;

	[SerializeField, Range( 0, 1 )]
	private int inputId_zRotation = 1;

	[SerializeField] private MinMax xRotationRange;
	[SerializeField] private MinMax zRotationRange;

    protected override void Update()
    {
		base.Update();

		Vector3 rotation = Vector3.zero;

		rotation.x = xRotationRange.GetValue( inputValues[ inputId_xRotation ].Precent );
		rotation.z = zRotationRange.GetValue( inputValues[ inputId_zRotation ].Precent );


		transform.eulerAngles = rotation;

    }
}
