using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public enum PancakeState {
	Mixture		= 0,
	Raw			= 1,
	Cooked		= 2,
	Perfect		= 3,
	OverCooked	= 4,
	Burnt		= 5,
	Fire		= 6,
	Count		= 7		// count of elements in enum
}

public class Pancake_state : MonoBehaviour, IBatterChanged
{

	public delegate void onStateChange( PancakeState state );
	public event onStateChange OnStateChanged;

	[SerializeField] private GameObject pancakeFire_prefab;
	[Tooltip("The ideal cooking temperature for 1:1 cooking time ratio")]
	[SerializeField] private float idealCookingTemperature = 175f;
	[SerializeField] private CookingState[] cookingStates;
	[Tooltip("The renderer must contain two materials on for each side of the pancake.")]
	[SerializeField] private Renderer pancakeRenderer;
	private Material[] pancakeMaterials;

	private int currentSide = -1;	//-1 == un inited.

	private PancakeState[] pancakeStates = new PancakeState[ 2 ];
	private Timer stateTimer = new Timer();
	private float remainingTime = 0;

	private void Start()
	{

		pancakeMaterials = pancakeRenderer.materials;

		// Register onto the batter changed callback so we can reset the state timer 
		// if batter is added while in a batter state.
		GetComponent<Batter_quantity>().OnBatterChanged += OnBatterChanged;

		// update both sides with a delta of 0 to make sure that the materials are up-to-date.
		// change to state 0 and update
		ChangeSideDown();
		UpdateState( 0 );
		// change to state 1 and update
		ChangeSideDown();
		UpdateState( 0 );
		// Change back to side 0
		ChangeSideDown();

		OnStateChanged?.Invoke( pancakeStates[ currentSide ] );
	}

	/// <summary>
	/// Updates the state timer
	/// </summary>
	/// <param name="delta"> the amount of time that has passed since the last update.</param>
	public void UpdateState( float panTemp )
    {
		// if the pancakes on fire we better get out of here (it might be a good idear to call the fire brigade)
		if ( stateTimer.IsCompleat && GetState() == PancakeState.Fire ) return;
		else if ( stateTimer.IsCompleat ) NextState();

		remainingTime = stateTimer.Update( ( panTemp / idealCookingTemperature ) * Time.deltaTime );

		UpdateMaterial();


    }

	private void UpdateMaterial()
	{

		// get the end color of the last cooking state. 
		// if its the mixture state it start on it own endColor so it does not change color at all.
		int startColorId = (int)pancakeStates[ currentSide ] == 0 ? 0 : (int)pancakeStates[ currentSide ] - 1;
		Color startColor = cookingStates[ startColorId ].endColor;
		Color endColor = cookingStates[ (int)pancakeStates[ currentSide ] ].endColor;

		// lerp between the start and end color updateing the material.
		pancakeMaterials[ currentSide ].color = Color.Lerp( startColor, endColor, stateTimer.TimerPrecentage() );
		
	}

	/// <summary>
	/// Get the current state of the side that is face down in the pan;
	/// </summary>
	/// <returns></returns>
	public PancakeState GetState()
	{
		return pancakeStates[ currentSide ];
	}

	/// <summary>
	/// Sets state of pancake that is face down in the pan.
	/// </summary>
	/// <returns> returns true if successful</returns>
	public bool SetState(PancakeState state)
	{
		// search for state and set it if not found do nothing

		foreach ( CookingState cState in cookingStates )
		{
			if ( cState.pancakeState == state )
			{
				pancakeStates[ currentSide ] = state;

				SetTimer();

				OnStateChanged?.Invoke( state );
				return true;
			}
		}

		return false;

	}

	/// <summary>
	/// Incress the state of pancake that is face down in the pan.
	/// </summary>
	/// <returns> returns true if successful</returns>
	public bool NextState()
	{

		if ( pancakeStates[ currentSide ] >= PancakeState.Count - 1 ) return false;

		// if the state is mixture update both states.
		if ( pancakeStates[ currentSide ] == PancakeState.Mixture)
		{
			//update both states.
			pancakeStates[ 0 ]++;
			pancakeStates[ 1 ]++;
		}
		else
		{
			pancakeStates[ currentSide ]++;

		}

		SetTimer();

		OnStateChanged?.Invoke( pancakeStates[ currentSide ] );

		return true;

	}

	public void ChangeSideDown()
	{

		currentSide = currentSide == 0 ? 1 : 0;

		SetTimer();

	}

	public void SetSideDown(int sideId)
	{

		if ( sideId == currentSide ) return; // nothing to update.

		ChangeSideDown();

	}

	public int GetSideDown()
	{
		return currentSide;
	}

	/// <summary>
	/// Reset the state timer for the current state.
	/// </summary>
	private void SetTimer()
	{
		// reset state length and add any ramining time from the last state.
		float timerLength = cookingStates[ (int)pancakeStates[ currentSide ] ].stateLength;

		stateTimer.SetTimer( timerLength, true );
		stateTimer.Update( remainingTime );			// Add the remaing time from the last state :)

		remainingTime = 0;

	}

	public void OnBatterChanged( float batterPercentage )
	{
		// if in mixture state reset the state timer.
		// we only have to check one side of the pancake 
		// since there are both mixture or nither of are mixture (cant be mixture state and another state)
		if ( pancakeStates[ 0 ] == PancakeState.Mixture )
			SetTimer();

	}

	//TODO: when the side changes i need to update the pancake states array

}

[System.Serializable]
public struct CookingState
{

	public PancakeState pancakeState;
	public float stateLength;
	public Color endColor;

	public void SetState( CookingState state)
	{
		pancakeState = state.pancakeState;
		stateLength = state.stateLength;
		endColor = state.endColor;
	}

	public bool CompareState( PancakeState state )
	{
		return pancakeState == state;
	}

}
