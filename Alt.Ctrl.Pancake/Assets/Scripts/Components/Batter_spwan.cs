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
	//private float currentBallQuantity = 0f;

	void Start()
    {
        
    }

    protected override void Update()
    {

		base.Update();

		if ( inputValue.ClampedPrecent == 1f )	//hmm this -1 ant good. it it cuz the min value is the max, but its not a grantee
		{
			if ( currentBatterBall != null ) currentBatterBall = null;

			return;
		}

		float spwanInterval = ( 1f / spwanFrequency.GetValue( 1f - inputValue.ClampedPrecent ) );

		if (Time.time >= nextSpwanTime)		// spwan a new batter ball 
		{

			currentBatterBall = Instantiate(batterBallPrefab, minSpwanLocation.position, Quaternion.identity);
			lastSpwanTime = Time.time;

		}
		else if(currentBatterBall != null)	//move the batter closer to the edge of the jug.
		{
			currentBatterBall.transform.position = Vector3.Lerp( minSpwanLocation.position, maxSpwanLocation.position, 1f - (nextSpwanTime - Time.time) / spwanInterval );

			float remainingBatter = currentBatterBall.AddBatter( jugBatterQuantity.UseBatter( maxPourRate * (1f - inputValue.ClampedPrecent) * Time.deltaTime ) );
			jugBatterQuantity.AddBatter(remainingBatter);

		}

		nextSpwanTime = lastSpwanTime +spwanInterval;

	}
}
