using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

/// <summary>
/// Base class for components that have a single input value
/// </summary>
public abstract class BasePanGroup_singleInput : BasePanGroup
{

	[Header( "Input" )]
	[SerializeField] private string inputName;
	[SerializeField] private bool addPanId_prefix = false;

	protected string InputName {
		get { return inputName + ( addPanId_prefix ? "_" + HobGroupId : "" ); }
	}
	[SerializeField] protected MinMax inputValue = new MinMax( 0, 1010 );

	protected virtual void Update()
	{

		UpdateInputValues();

	}

	protected virtual void UpdateInputValues()
	{

		Inputs.GetInputValue( InputName, ref inputValue.current );
		inputValue.current += inputValueOffset;

	}

}
