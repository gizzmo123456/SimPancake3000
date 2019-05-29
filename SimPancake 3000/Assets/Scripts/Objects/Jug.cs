using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Jug : MonoBehaviour
{

	[Header("Inputs and limits")]
	[SerializeField] private MinMax minMaxInputValue = new MinMax( -10, 90 );
	[SerializeField] private MinMax minMaxRotation = new MinMax( -10, 90 );

	[Header("Batter")]
	[SerializeField] private Transform batter;
	private float batter_amount = 1f;
	[SerializeField] private float batter_maxPourRate = 0.2f;   // per second
	[SerializeField] private MinMax batter_yScale_pour = new MinMax(0.2f, 1f);

	[SerializeField] private BatterTrail batterTrail;
	[SerializeField] private Transform pourTrail_startPosition;
	[SerializeField] private Transform pourTrail_lerpEndPosition;
	[SerializeField] private float batterTrail_spwIntervals = 0.25f;
	private float batter_nextSpwTime = 0;
	private float batterTrail_pourAmount = 0f;			// the total amount of batter that has been paourd within the spwan intervals.

	[Header( "Position" )]
	private int currentPosition = -1; // <0 is difault position.

	[SerializeField] private Vector3 defaultPosition = Vector3.zero;
	[SerializeField] private Transform[] fryingPans;
	[SerializeField] private Vector3 fryingPanOffset;

	private float panToggle_releaseTime = -1;
	[SerializeField] private float panToggle_thresshold = 0.5f;


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		InputValues inputs = InputHandler.GetInputs();

		SelectPan( inputs.panToggle );

		// we can not pour when in the default postion
		if ( currentPosition < 0 ) return;

		Vector3 rotation = transform.eulerAngles;

		minMaxInputValue.current = inputs.jug;
		rotation.x = GetCurrentXRotation();

		transform.eulerAngles = rotation;

		// keep the batters x rotation @ 0 so its always level.
		// TODO: make dynamic.
		Vector3 batterRotation = batter.eulerAngles;
		Vector3 batterScale = batter.localScale;

		batterRotation.x = 0;
		batter.eulerAngles = batterRotation;

		batterScale.y = batter_yScale_pour.GetValue( minMaxInputValue.Precent );

		batter.localScale = batterScale;

		//TEST.
		// spwan batter pour
		if(Mathf.Abs(rotation.x) > 10 && Time.time >= batter_nextSpwTime)
		{
			BatterTrail bTrail = Instantiate( batterTrail , pourTrail_startPosition.position, Quaternion.identity );
			bTrail.Init(this, pourTrail_startPosition, pourTrail_lerpEndPosition, batterTrail_pourAmount);
			batter_nextSpwTime = Time.time + batterTrail_spwIntervals;
			batterTrail_pourAmount = 0;
		}
		else if( Mathf.Abs( rotation.x ) > 10 && Time.time < batter_nextSpwTime )
		{
			batterTrail_pourAmount += (batter_maxPourRate * minMaxInputValue.Precent) * Time.deltaTime;
		}

	}

	private void SelectPan(float selectPan_input)
	{
		selectPan_input = 1f - selectPan_input;

		if ( selectPan_input > 0 && ( Time.time <  panToggle_releaseTime + panToggle_thresshold ||															// cycel through pans, be pressed befor the end of the thresshold
									( Time.time >= panToggle_releaseTime + panToggle_thresshold && currentPosition == -1 ) ))	// or we're in the default position and pressed after the thresshold
		{                                                                                                                       

			currentPosition++;

			if ( currentPosition >= fryingPans.Length )
				currentPosition = 0;

			panToggle_releaseTime = -1;	
		}
		else if(selectPan_input == 0 && panToggle_releaseTime < 0)																// toggle released
		{
			panToggle_releaseTime = Time.time;
		}
		else if( currentPosition != -1 && selectPan_input == 0 && Time.time >= panToggle_releaseTime + panToggle_thresshold )	// reset to default be release for longer than thresshold
		{
			currentPosition = -1;
		}


		if ( currentPosition < 0 )
			transform.position = defaultPosition;
		else
			transform.position = fryingPans[ currentPosition ].position + fryingPanOffset;
		
	}

	public float GetCurrentXRotation()
	{
		return minMaxRotation.GetValue( minMaxInputValue.Precent );
	}

}
