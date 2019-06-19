using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Batter_spwan : BasePanGroup_singleInput
{
	[Header("Batter Spwan")]
	[SerializeField] private Batter_quantity jugBatterQuantity;

	[SerializeField] private Transform minSpwanLocation;
	[SerializeField] private Transform maxSpwanLocation;

	[Tooltip( "Max Spwan Frequency Per Second" )]
	[SerializeField] private MinMax spwanFrequency = new MinMax( 0.5f, 2f );

	private float nextSpwanTime = 0;
	private float lastSpwanTime = 0;

	[Tooltip("Units of batter per second")]
	[SerializeField] private float maxPourRate = 1f;

	[SerializeField] private Batter_quantity batterBallPrefab;   //this will need to be change to a batter ball call, i still need to do it.
	private Batter_quantity currentBatterBall;
	[SerializeField] private float batterBall_trail_minLife = 0.75f;

	bool active = false;

	private void Start()
	{
		GetComponent<Jug_panSellect>().OnPanChanged += OnPanChanged;
	}

	protected override void Update()
    {

		base.Update();

		if ( !active ) return;

		if ( inputValue.ClampedPrecent == 1f )	//hmm this -1 ant good. it it cuz the min value is the max, but its not a grantee
		{
			if ( currentBatterBall != null )
			{
				currentBatterBall.SendMessage( "OnRelease" );
				currentBatterBall = null;
			}

			return;
		}

		float spwanInterval = ( 1f / spwanFrequency.GetValue( 1f - inputValue.ClampedPrecent ) );

		if (Time.time >= nextSpwanTime)		// spwan a new batter ball 
		{

			// Let the current ball know its being released from the jug.
			currentBatterBall?.SendMessage("OnRelease");

			currentBatterBall = Instantiate(batterBallPrefab, minSpwanLocation.position, Quaternion.identity);
			lastSpwanTime = Time.time;

			// set batter ball trails lifetime :)
			float batterTrailLife = spwanInterval < batterBall_trail_minLife ? batterBall_trail_minLife : spwanInterval;
			currentBatterBall.SendMessage( "SetTime", batterTrailLife );

		}
		else if(currentBatterBall != null)	//move the batter closer to the edge of the jug.
		{

			currentBatterBall.transform.position = Vector3.Lerp( minSpwanLocation.position, maxSpwanLocation.position, 1f - (nextSpwanTime - Time.time) / spwanInterval );

			float remainingBatter = currentBatterBall.AddBatter( jugBatterQuantity.UseBatter( maxPourRate * (1f - inputValue.ClampedPrecent) * Time.deltaTime ) );
			jugBatterQuantity.AddBatter(remainingBatter);	// return any batter that could not be added to the ball back to the jug :P

		}

		nextSpwanTime = lastSpwanTime +spwanInterval;

	}

	private void OnPanChanged( int id )
	{
		active = id > -1;
	}

}
