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
		if ( inputValues.Length == inputNames.Length )
			Debug.LogError( GameGlobals.GetLogPrefix(name, GetType().ToString())+"Input put values and input names must be the same size" );
	}

	protected virtual void Update()
	{

		for ( int i = 0; i < inputNames.Length; i++ )
			Inputs.GetInputValue( GetInputName( i ), ref inputValues[ i ].current );

	}


}
