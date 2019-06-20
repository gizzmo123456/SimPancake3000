using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

/// <summary>
/// Scales the pancake by its volume (quantity) when in a mixture state.
/// based on the volume of a cylinder :D :
/// </summary>
[RequireComponent(typeof(Pancake_state))]
[RequireComponent(typeof(Batter_quantity))]
public class Pancake_scale : MonoBehaviour, IPancakeStateChanged, IBatterChanged
{

	private bool active = true;	// we aleays start in a mixture state. (this will only change if i start pooling pancakes, but that unnessary atm)
	private Batter_quantity batterQuantity;

	[SerializeField] private float spreadRate = 1f;
	[SerializeField] private MinMax heightRange = new MinMax(0.1f, 1f);
	private float radius = 0.05f;
	[SerializeField] private float maxRadius = 1.6f;

	private float Height {
		get { return batterQuantity.GetBatterQuantity() / ( Mathf.PI * Mathf.Pow( radius, 2 ) ); }
	}

	private Vector3 startLerpScale;
	private Vector3 targetScale;    // we add batter in intervals, so we need to lerp to make it smoth

	private Timer lerpTimer = new Timer();
	[SerializeField] private float lerpIntervals = 0.5f;

    void Awake()
    {

		GetComponent<Pancake_state>().OnStateChanged += OnPancakeStateChanged;
		batterQuantity = GetComponent<Batter_quantity>();

		//NOTE: just for now.
		// I WILL NOT be using scale but just to get it working.
		// insted i should move the bones/joints of the pnacake.
		// (cuz pancakes have bones now :D)
		transform.localScale = startLerpScale = targetScale = Vector3.zero;

    }

    void Update()
    {

		// make shore that the lerp is finished, so the pancake is fully spread.
		if ( !active && lerpTimer.IsCompleat) return;

		SpreadBatter();
		LerpScale();

    }

	private void SpreadBatter()
	{
		// spread the pancake out while its mixture
		// based on volume of cylinder

		heightRange.current = Height;

		if ( targetScale.y > heightRange.min )
			radius += spreadRate * heightRange.Precent * Time.deltaTime;

		if ( radius > maxRadius ) radius = maxRadius;

		targetScale.y = Height > heightRange.min ? Height : heightRange.min;
		targetScale.x = radius / 2f;
		targetScale.z = radius / 2f;

	}

	private void LerpScale()
	{
		lerpTimer.Update( Time.deltaTime);

		transform.localScale = Vector3.Lerp( startLerpScale, targetScale, ( lerpTimer.TimerPrecentage(true) ) );
	}

	public void OnPancakeStateChanged(PancakeState state)
	{
		active = state == PancakeState.Mixture;
	}

	public void OnBatterChanged( float batterPercent )
	{

		startLerpScale = transform.localScale;
		lerpTimer.SetTimer( lerpIntervals, true );

	}
}
