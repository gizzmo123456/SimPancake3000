using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;


public class FryingPan_hobPosition : BasePanGroup_singleInput
{
	[Header("Hob Position")]
	[SerializeField] private MinMax yPositionRange = new MinMax( 0, 10 );

    protected override void Update()
    {

		base.Update();

		Vector3 position = transform.position;

		position.y = yPositionRange.GetValue( inputValue.Precent );

		transform.position = position;

    }
}
