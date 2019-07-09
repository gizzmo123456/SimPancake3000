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

public class Pancake_state : MonoBehaviour, IBatterChanged, IChild
{

	public delegate void onStateChange( PancakeState state );
	public delegate void onSideChanged( int sideId );
	public event onStateChange OnStateChanged;
	public event onSideChanged OnSideChanged;

	[SerializeField] private GameObject pancakeFire_prefab;
	[SerializeField] private Vector3 fire_localOffset;
	[SerializeField] private float burn_idealThinkness = 0.15f;

	[Tooltip("The ideal cooking temperature for 1:1 cooking time ratio")]
	[SerializeField] private float idealCookingTemperature = 175f;
	[SerializeField] private CookingState[] cookingStates;
	[Tooltip("The renderer must contain two materials on for each side of the pancake.")]
	[SerializeField] private Renderer pancakeRenderer;
	private Material[] pancakeMaterials;

	private int currentSideDown = -1;	//-1 == un inited.

	private PancakeState[] pancakeStates = new PancakeState[ 2 ];
	private Timer[] stateTimer = new Timer[ 2 ];
	private float remainingTime = 0;

	private bool caughtFire = false;
	private bool isChild = false;
	[Range(0f, 0.75f)]
	[SerializeField] private float isChildCookingSpeed = 0.1f;

	private void Awake()
	{
		// init our tiemrs for each side of the pancake
		stateTimer[ 0 ] = new Timer();
		stateTimer[ 1 ] = new Timer();

		// Register onto the batter changed callback so we can reset the state timer 
		// if batter is added while in a batter state.
		GetComponent<Batter_quantity>().OnBatterChanged += OnBatterChanged;

		pancakeMaterials = pancakeRenderer.materials;

		// update both sides with a delta of 0 to make sure that the materials are up-to-date.
		// change to side 0 and update
		ChangeSideDown();
		UpdateState( 0 );
		// change to side 1 and update
		ChangeSideDown();
		UpdateState( 0 );

		// Change back to side 0
		ChangeSideDown();
	}

	private void Start()
	{
		OnStateChanged?.Invoke( pancakeStates[ currentSideDown ] );
	}

	/// <summary>
	/// Updates the state timer
	/// </summary>
	/// <param name="delta"> the amount of time that has passed since the last update.</param>
	public void UpdateState( float panTemp )
    {
		// if the pancakes on fire we better get out of here (it might be a good idear to call the fire brigade)...
		if ( IsOnFire() )
		{
			// If your not on fire, i demand you to be so.
			if ( !caughtFire )
				CatchFire();

			return;
		}
		
		// if the current side down is >= a burnt state, slowly cook the side up, since it does burn throught affter a while in the real world.
		// once the pancake has a fire side and a side at, atleast burnt state trigger fire, also along this we need to trigger thick
		// black smoke to indercate that the pancake is about to catch fire.
		if ( GetState() >= PancakeState.Burnt ) CookOtherSide( panTemp );

		if ( stateTimer[ currentSideDown ].IsCompleat && GetState() != PancakeState.Fire ) NextState();

		float updateAmount = ( panTemp / idealCookingTemperature ) * Time.deltaTime;

		if ( isChild )
			updateAmount *= isChildCookingSpeed;

		remainingTime = stateTimer[ currentSideDown ].Update( updateAmount );
		UpdateMaterial( currentSideDown );

    }

	private void CookOtherSide( float panTemp )
	{

		// cookes the other side once the current side down has reached a burnt state,
		// affected by the thickness of the pancake.

		float thinkness = transform.localScale.y / burn_idealThinkness;  
		int upSideId = GetSideUp();

		float updateAmount = ( panTemp / idealCookingTemperature ) * thinkness * Time.deltaTime;

		if ( isChild )
			updateAmount *= isChildCookingSpeed;

		float remainTime = stateTimer[ upSideId ].Update( updateAmount );

		if ( stateTimer[ upSideId ].IsCompleat && pancakeStates[upSideId] < PancakeState.Fire )
		{
			pancakeStates[ upSideId ]++;
			SetTimer( upSideId, remainingTime );
		}

		UpdateMaterial( upSideId );

	}

	private void UpdateMaterial( int sideId )
	{

		// get the end color of the last cooking state. 
		// if its the mixture state it start on it own endColor so it does not change color at all.
		int startColorId = (int)pancakeStates[ sideId ] == 0 ? 0 : (int)pancakeStates[ sideId ] - 1;
		Color startColor = cookingStates[ startColorId ].endColor;
		Color endColor = cookingStates[ (int)pancakeStates[ sideId ] ].endColor;

		// lerp between the start and end color updateing the material.
		pancakeMaterials[ sideId ].color = Color.Lerp( startColor, endColor, stateTimer[ sideId ].TimerPrecentage() );
		
	}

	public bool IsOnFire()
	{

		bool downSide = GetState( true ) == PancakeState.Fire && GetState( false ) >= PancakeState.OverCooked;
		bool upSide = GetState( true ) >= PancakeState.OverCooked && GetState( false ) == PancakeState.Fire;

		return downSide || upSide;

	}

	/// <summary>
	/// Get the current state of the side that is face down in the pan;
	/// </summary>
	/// <returns></returns>
	public PancakeState GetState( bool down = true )
	{

		return pancakeStates[ down ? currentSideDown : GetSideUp() ];
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
				pancakeStates[ currentSideDown ] = state;

				SetTimer( currentSideDown );

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

		if ( pancakeStates[ currentSideDown ] >= PancakeState.Count - 1 ) return false;

		// if the state is mixture update both states.
		if ( pancakeStates[ currentSideDown ] == PancakeState.Mixture)
		{
			//update both states.
			pancakeStates[ 0 ]++;
			pancakeStates[ 1 ]++;

			// when going from mixture to raw state we must update the upside timer as well as the down side.
			SetTimer( GetSideUp(), 0 );	// do down side below, it need to be done for all state changes.

		}
		else
		{
			pancakeStates[ currentSideDown ]++;

		}

		SetTimer( currentSideDown );

		OnStateChanged?.Invoke( pancakeStates[ currentSideDown ] );

		return true;

	}

	public void ChangeSideDown()
	{

		currentSideDown = currentSideDown == 0 ? 1 : 0;

		OnSideChanged?.Invoke( currentSideDown );

		Debug.Log( "[" + name + "] Side Down Changed: " + currentSideDown, gameObject );


		// SetTimer();		// we no longer need to reset the state timer if there are both sides have a timer.
		// it only has to be done when the state changes.

	}

	public void SetSideDown(int sideId)
	{

		Debug.Log( "["+name+"] Set Side Down: "+sideId+" ( "+ ( sideId == currentSideDown ) + ")", gameObject );

		if ( sideId == currentSideDown ) return; // nothing to update.

		ChangeSideDown();

	}

	public int GetSideDown()
	{
		return currentSideDown;
	}

	public int GetSideUp()
	{
		return 1 - currentSideDown;
	}

	/// <summary>
	/// Reset the state timer for the current state.
	/// </summary>
	/// <param name="updateDelta"> amount of time to update the time with less than 0 will use any remaing time  </param>
	private void SetTimer( int sideId, float updateDelta = -1 )
	{
		// reset state length and add any ramining time from the last state.
		float timerLength = cookingStates[ (int)pancakeStates[ sideId ] ].stateLength;

		stateTimer[ sideId ].SetTimer( timerLength, true );

		if ( updateDelta < 0 ) // Add the remaing time from the last state :)
		{
			updateDelta = remainingTime;
			remainingTime = 0;
		}

		stateTimer[ sideId ].Update( updateDelta );          


	}

	public void CatchFire()
	{

		// check to see if the pancake is on fire.
		// spawn fire and sound the alarm.
		if ( !IsOnFire() || caughtFire ) return;

		GameObject fire;

		if ( pancakeFire_prefab != null )
		{
			// spawn then make child so it is orientated corrently
			fire = Instantiate( pancakeFire_prefab, transform.position, Quaternion.identity );
			fire.transform.parent = transform;
		}

		// Send message to arduino via serial. to sound the alarm.
		// if the game is not muted 
		if(!GameGlobals.mute)
			GameGlobals.inputs.Serial_queueWriteLine("f");

		caughtFire = true;

		// Fire....
		// Alarm...
		// Visual feedback...
		// ??? 
	}

	public void OnBatterChanged( float batterPercentage )
	{
		// if in mixture state reset the state timer.
		// we only have to check one side of the pancake 
		// since there are both mixture or nither of are mixture (cant be mixture state and another state)
		if ( pancakeStates[ 0 ] == PancakeState.Mixture )
			SetTimer( currentSideDown );

	}

	public void SetIsChild( bool isChi )
	{
		isChild = isChi;
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
