﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan_pancake : BasePanGroup, IReceivePancake
{

	protected List<Pancake_state> pancakes; // all the pancakes that are current in this pan :)
	/// <summary>
	/// Get the amount of pancakes that are in this pan.
	/// </summary>
	public int PancakeCount {
		get { return pancakes.Count; }
	}

	[SerializeField] private Pancake_state pancakePrefab;
	[SerializeField] private Transform panColliderObj;

	private void Start()
	{

		pancakes = new List<Pancake_state>();

	}

	public void AddBatter( float qt , Vector3 hitLocaltion )
	{

		// if there no pancakes with the state of mixture in the pan creat a new pancake.
		// if there are pancakes but not in the state of mixture combine them into the new pancake.
		// else add the batter to the current mixture pancake.
		if ( !IsMixturePancake() )	// Create new pancake
		{
			Pancake_state pancake = Instantiate( pancakePrefab, hitLocaltion, Quaternion.identity );
			pancake.GetComponent<Batter_quantity>().AddBatter(qt);
			SendPanMessage(pancake);

			// if there are already pancake in the pan combine them into the new pancake.
			// TODO: this should only be if the mixture coms into contact this a non mixture pancake.
			// TODO: Also this states should be set onto this pancake??
			for(int i = 0; i < pancakes.Count; i++ )
			{
				pancakes[ i ].gameObject.AddComponent<Pancake_child>().SetParent( pancake.transform );
			}

			// Clear the list and add the new pancake, since it's now the only pancake in the pan.
			// and add the new pancake
			pancakes.Clear();
			pancakes.Add( pancake );

		}
		else						// add mixture to pancake
		{
			pancakes[ 0 ].GetComponent<Batter_quantity>().AddBatter(qt);//there can only be 1 pancake 
		}
		

	}

	public void CookPancakes( float panTempture )
	{

		foreach (Pancake_state pancake in pancakes)
			pancake.UpdateState( panTempture );

	}

	public bool IsMixturePancake()
	{

		for ( int i = 0; i < pancakes.Count; i++ )
			if ( pancakes[ i ].GetState() == PancakeState.Mixture )
				return true;

		return false;
	}

	private void SendPanMessage(Pancake_state pancake)
	{

		IPanCollider[] panCol = pancake.GetComponents<IPanCollider>();

		foreach ( IPanCollider pan in panCol )
			pan.SetPanCollider( panColliderObj );

	}

	public void AddPancake( Pancake_state pancake )
	{
		// TODO: if theres mixture in pan, combine. 
		pancakes.Add( pancake );
	}

	public void RemovePancake ( Pancake_state pancake )
	{
		pancakes.Remove( pancake );
	}

	public Pancake_state GetPancake( int id )
	{

		return pancakes[ id ];
	}

	public Pancake_state[] GetPancakes()
	{
		return pancakes.ToArray();
	}

}
