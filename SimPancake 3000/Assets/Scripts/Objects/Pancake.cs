using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof(Rigidbody))]
public class Pancake : MonoBehaviour
{

    private Rigidbody rb;
	private Material[] materials;	// there should be a material for each side.
    private FryingPan currentPan;   //null if its not in a pan

	private Vector3 startPosition;

	// lerp
	private Vector3 startLerpScale = Vector3.zero;
	private Vector3 targetScale = Vector3.zero;
	private float targetLerpLength = 0.5f;
	private float targetLerpTime = 0f;

	// pancake size and spread
	private float volume = 0;
	private float radius = 0.1f;
	[SerializeField] private float maxRadius = 1f;
	[SerializeField] private MinMax heightRange = new MinMax(0.1f, 1f);

	private float Height
	{
		get{ return volume / ( Mathf.PI * Mathf.Pow( radius, 2 ) ); }
	}

	[SerializeField] private float spreadRate = 0.2f;

	// cooking
	[SerializeField] private float idealCookingTemperature = 150f;
	private float currentCookingTemperature = 0f;

	[Tooltip("This should be set up in the order of states.")]
	[SerializeField] private CookingState[] cookingStates;
	private int currentSide = 0;
	private CurrentCookingState[] currentCookingStates = new CurrentCookingState[ 2 ];	//there are two state one for each side of the pancake :)
	
	//Debug and errors
	private bool error = false;
	private bool debug = false;

	private void Awake()
	{

		materials = GetComponent<MeshRenderer>().materials;

		transform.localScale = Vector3.zero;    //New pancakes should have a size of zero :).

		if( cookingStates.Length == 0 )
		{
			error = true;
			Debug.LogError("Cooking states have not been setup :( ");
			return;
		}

		/*
		 * Set both sides of the pancake to the first state;
		 *
		 * basicly this works by starting both sides of the pancake on the first cooking state,
		 * once that state has finished both will move on to the next state,
		 * affter that both sides of the pancake become inderpendent of each other.
		 * 
		 */
		currentCookingStates[ 0 ].NewTimer();
		SetNextState( 0, 0 );
		UpdatePancakeColor( 0 );

		currentCookingStates[ 1 ].NewTimer();
		SetNextState( 1, 0 );
		UpdatePancakeColor( 1 );

	}

	void Start()
    {
        rb = GetComponent<Rigidbody>();
	}
	
    void Update()
    {
		
		if ( error ) return;

        WakeUp();
		UpdateCookingState();

		if( currentCookingStates[ currentSide ].currentState.pancakeState == PancakeState.Mixture )
		{

			SpreadPancakeBatter();
			LerpPancakeSize();

			//since we're in the mixture state we can assume that the frying pan is the parent of the pancake :)
			float yPositionOffset = ( ( transform.localScale.y / 2f ) * transform.parent.localScale.y );
			transform.localPosition = startPosition + new Vector3( 0, yPositionOffset, 0 );

			transform.localEulerAngles = new Vector3( 0, 90, 0 );
		}

    }

	private void SpreadPancakeBatter() 
	{

		heightRange.current = Height;

		if(targetScale.y > heightRange.min)
			radius += spreadRate * heightRange.Precent * Time.deltaTime;
		
		if ( radius > maxRadius ) radius = maxRadius;

		targetScale.y = Height > heightRange.min ? Height : heightRange.min;
		targetScale.x = radius / 2f;
		targetScale.z = radius / 2f;

	}

	private void LerpPancakeSize()
	{
		targetLerpTime += Time.deltaTime;

		if ( targetLerpTime > targetLerpLength )
			targetLerpTime = targetLerpLength;

		transform.localScale = Vector3.Lerp( startLerpScale, targetScale, ( targetLerpTime / targetLerpLength ) );

	}

    public void SetCurrentPan(FryingPan fryingPan)
    {
        currentPan = fryingPan;
		GetComponent<PancakeFlip>().SetFryingPan( currentPan );
	}

	public void WakeUp()
    {
        if (rb.IsSleeping())
        {
            print("Stop sleeping on the job!");
            rb.WakeUp();
        }
    }
/*
	public void AddForce(float force)
	{
		//rb.velocity = new Vector3( 0, force, 0 );// AddForce( new Vector3( 0, force, 0 ) );
		//SendMessage( "SetTorque", force );
		//rb.AddTorque(new Vector3(-1500f * force, 0, 0) );
		rb.AddForce( currentPan.transform.right * force * 2f, ForceMode.VelocityChange);
	}
*/
	public void AddBatter( float batterQt, float intervalLength )
	{

		//check we are in a raw state.
		if( currentCookingStates[ currentSide ].currentState.pancakeState != PancakeState.Mixture )
		{
			Debug.LogError( "Can only add batter to a raw pancake." );
			return;
		}

		startLerpScale = transform.localScale;
		volume += batterQt;
		targetLerpLength = intervalLength;
		targetLerpTime = 0;

		// reset the cooking time.
		SetNextState( 0, 0 );
		SetNextState( 1, 0 );

	}

	public void SetStartPosition(Vector3 worldStartPos)
	{
		transform.position = worldStartPos;
		startPosition = transform.localPosition;
	}

	/// <summary>
	/// Returns the state of the current side, unless side is supplied
	/// </summary>
	public PancakeState GetCurrentState( int side = -1)
	{
		if( side < 0 || side >= currentCookingStates.Length)
			return currentCookingStates[ currentSide ].currentState.pancakeState;
		else
			return currentCookingStates[ side ].currentState.pancakeState;

	}

	private void UpdateCookingState()
	{
		// Do not cook unless we are in a pan :)
		if ( currentPan == null ) return;

		float time = ( Time.deltaTime * ( currentCookingTemperature / idealCookingTemperature ) ); // should this take height into account??
		currentCookingStates[ currentSide ].stateTimer.Update( time );

		if( currentCookingStates[ currentSide ].stateTimer.IsCompleat )
		{
			//Update both sides if state id is 0 then they are inderpendent after.
			if( currentCookingStates[currentSide].currentCookingStateId == 0)
			{
				SetNextState( 0 );
				UpdatePancakeColor( 0 );
				SetNextState( 1 );
				UpdatePancakeColor( 1 );

			}
			else
			{
				SetNextState( currentSide );
				UpdatePancakeColor( currentSide );
			}

		}
		else
		{
			UpdatePancakeColor( currentSide );
		}

	}

	private void UpdatePancakeColor(int side)
	{
		int stateId = currentCookingStates[ side ].currentCookingStateId;

		Color startColor =  stateId == 0 ? cookingStates[ 0 ].endColor : cookingStates[ stateId - 1 ].endColor;
		Color endColor = cookingStates[ stateId ].endColor;

		materials[ side ].color = Color.Lerp( startColor, endColor, currentCookingStates[ side ].stateTimer.TimerPrecentage() );

	}

	/// <summary>
	/// Set the next state for a side of pancake :).
	/// </summary>
	/// <param name="side"> the side of pancake to move onto the next state </param>
	/// <param name="nextStateId"> the next state id, if >= 0 to manuly set the cooking state else auto (default -1) </param>
	private void SetNextState(int side, int nextStateId = -1)
	{
		if( nextStateId < 0)
			nextStateId = currentCookingStates[ side ].currentCookingStateId + 1;

		if ( nextStateId >= cookingStates.Length ) return;

		CookingState state = cookingStates[ nextStateId ];

		currentCookingStates[ side ].currentCookingStateId = nextStateId;
		currentCookingStates[ side ].currentState = state;  //Nedded ?
		currentCookingStates[ side ].stateTimer.SetTimer( state.stateLength, true );

	}
	
	public void SetTemperature(float temp)
	{
		currentCookingTemperature = temp;
	}
}

public enum PancakeState	//Use State?
{
	Mixture = 0,
	Raw		= 1,
	Cooked	= 2,
	Burnt	= 3,
	Fire	= 4
}

[System.Serializable]
public struct CookingState
{

	public PancakeState pancakeState;
	public float stateLength;
	public Color endColor;

	public void SetState(PancakeState state)
	{
		pancakeState = state;
	}

	public bool CompareState(PancakeState state)
	{
		return pancakeState == state;
	}

}

[System.Serializable]
public struct CurrentCookingState
{
	public int currentCookingStateId;
	public CookingState currentState;
	public Timer stateTimer;

	public void NewTimer()
	{
		stateTimer = new Timer();
	}
}