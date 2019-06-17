using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

/// <summary>
/// Base class for components that have multiple inputs.
/// 
/// </summary>
public abstract class BasePanGroup_multipleInput : BasePanGroup
{
	protected abstract int RequiredInputs { get; }
	protected bool error = false;

	[Header( "Inputs" )]
	[SerializeField] private string[] inputNames;
	[SerializeField] private bool addPanId_prefix = false;
	[SerializeField] protected MinMax[] inputValues;


	protected string GetInputName( int inputId )
	{
		return inputNames[inputId] + ( addPanId_prefix ? "_" + HobGroupId : "" );
	}

	protected virtual void Awake()
	{
		if (inputNames.Length != RequiredInputs )
		{
			error = true;
			Debug.LogError( GameGlobals.GetLogPrefix( name, GetType().ToString() ) + " Component Requires exactly "+RequiredInputs+" Inputs ", this );
		}

		if ( inputValues.Length != RequiredInputs )
		{
			error = true;
			Debug.LogError( GameGlobals.GetLogPrefix( name, GetType().ToString() ) + "Input put values and input names must be the same size", this );
		}

	}

	protected virtual void Update()
	{
		// update inputs if no error :)
		if( !error )
			for ( int i = 0; i < inputNames.Length; i++ )
				Inputs.GetInputValue( GetInputName( i ), ref inputValues[ i ].current );

	}


}
