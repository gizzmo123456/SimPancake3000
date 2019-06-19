using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Jug_rotation : BasePanGroup_singleInput
{
	[Header("Jug Position")]
	[SerializeField] private MinMax xRotationRange;

    protected override void Update()
    {

		base.Update();

		Vector3 rotation = Vector3.zero;

		rotation.y = -90;	
		rotation.x = xRotationRange.GetValue( inputValue.Precent );

		transform.eulerAngles = rotation;

    }
}
