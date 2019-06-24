﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PancakeState { Mixture, Raw, Cooked, Perfect, OverCooked, Burnt, Fire }

public class Pancake_state : MonoBehaviour
{

	public delegate void onStateChange( PancakeState state );
	public event onStateChange OnStateChanged;

	private PancakeState pancakeState = PancakeState.Mixture;

	private void Start()
	{
		//
		OnStateChanged?.Invoke(pancakeState);

	}

	void Update()
    {
        // state logic?
    }

	public PancakeState GetState()
	{
		return pancakeState;
	}

	public void SetState(PancakeState state)
	{
		pancakeState = state;

		OnStateChanged?.Invoke(state);

	}

}

