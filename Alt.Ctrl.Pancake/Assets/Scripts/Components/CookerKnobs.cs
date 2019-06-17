using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class CookerKnobs : BasePanGroup_singleInput
{

	[Tooltip("Start (min) and End (max) rotation of the hobKnob")]
	[SerializeField] private MinMax rotationRange;



	protected override void Update()
	{

		base.Update();
		
		Vector3 rotation = transform.eulerAngles;

		rotation.z = rotationRange.GetValue( inputValue.Precent );

		transform.eulerAngles = rotation;

	}


}
