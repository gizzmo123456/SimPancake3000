using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan_pancake : BasePanGroup
{

	protected List<Pancake_state> pancakes;	// all the pancakes that are current in this pan :)
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
		if ( !HasMixturePancake() )	// Create new pancake
		{
			Pancake_state pancake = Instantiate( pancakePrefab, hitLocaltion, Quaternion.identity );
			pancake.GetComponent<Batter_quantity>().AddBatter(qt);
			pancake.GetComponent<Pancake_jointSetup>().SetPanCollider(panColliderObj);

			// if there are already pancake in the pan combine them into the new pancake.
			// TODO: this should only be if the mixture coms into contact this a non mixture pancake.
			// TODO: Also this states should be set onto this pancake??
			for(int i = 0; i < pancakes.Count; i++ )
			{
				pancakes[ i ].transform.parent = pancake.transform;
			}

			// Clear the list and add out new pancake, since it's now the only pancake in the pan.
			// and add the new pancake
			pancakes.Clear();
			pancakes.Add( pancake );

		}
		else						// add mixture to pancake
		{
			pancakes[ 0 ].GetComponent<Batter_quantity>().AddBatter(qt);//there can only be 1 pancake 
		}
		

	}

	private bool HasMixturePancake()
	{

		for ( int i = 0; i < pancakes.Count; i++ )
			if ( pancakes[ i ].GetState() == PancakeState.Mixture )
				return true;

		return false;
	}

}
