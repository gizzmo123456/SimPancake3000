using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent( typeof( SkinnedMeshRenderer ) )]
public class Jug_batterWobBlendShape : BasePanGroup_singleInput, IPanChanged
{
	private bool active = false;
	[Range( 0.25f, 3f )]
	[SerializeField] float inputMultiplier = 1f;
	private SkinnedMeshRenderer skinnedMeshRenderer;

	[Header( "Woble BlendShape" )]
	[SerializeField] private int[] pour_blendShapeIds;
	private float[] blendShapeWeights;

	private float currentVelocity = 0;
	[SerializeField] private float maxVelocity;
	[SerializeField] private float counterVelocity;
	[SerializeField] private AnimationCurve velocityCurve;

	private MinMax blendAmount = new MinMax( 0, 100 );
	private bool positive = false;

	private float lastInputValue = 0;

	private void Awake()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		blendShapeWeights = new float[ pour_blendShapeIds.Length ];
		NewBlendWeights();
	}

	private void Start()
	{
		// Add Panchanged Callback.
		GetComponentInParent<Jug_panSellect>().OnPanChanged += OnPanChanged;
	}

	protected override void Update()
    {

		if ( active )
		{	// only update the inputs when active. so the wobble doesnt just stop.
			base.Update();
		}

		UpdateBlendAmount();

		// Update current velocity from our input value
		float inputDelta = Mathf.Abs( inputValue.current - lastInputValue ) * inputMultiplier;

		if ( inputDelta > currentVelocity )
			currentVelocity += inputDelta;

		if ( currentVelocity > maxVelocity ) currentVelocity = maxVelocity;

		// loop blend shape ids and set there new value
		for (int i = 0; i < pour_blendShapeIds.Length; i++ )
		{
			skinnedMeshRenderer.SetBlendShapeWeight( pour_blendShapeIds[ i ], blendAmount.current * blendShapeWeights[i]);
		}

		// work out the velocity for the next frame
		if ( currentVelocity > 0 )
		{
			currentVelocity -= counterVelocity * velocityCurve.Evaluate( currentVelocity / maxVelocity ) * Time.deltaTime;
		}
		else if ( currentVelocity < 0.01f )
		{
			currentVelocity = 0;
			blendAmount.current = 0;
		}

		blendAmount.max = currentVelocity / maxVelocity * 100f;

	}

	private void UpdateBlendAmount()
	{

		if ( currentVelocity == 0 ) return; // nuffin to update

		if ( positive )
			blendAmount.current += currentVelocity * Time.deltaTime;
		else
			blendAmount.current -= currentVelocity * Time.deltaTime;

		// flip blend direction
		if ( blendAmount.current >= blendAmount.max && positive )
		{
			positive = false;
			blendAmount.current = blendAmount.max;
		}
		else if ( blendAmount.current <= blendAmount.min && !positive )
		{
			positive = true;
			blendAmount.current = blendAmount.min;
			NewBlendWeights();
		}

	}

	private void NewBlendWeights()
	{

		for ( int i = 0; i < blendShapeWeights.Length; i++ )
			blendShapeWeights[ i ] = Random.Range(0.25f, 1f);

	}

	public void OnPanChanged( int panId )
	{
		active = panId > -1;

		// reset the input to 0 if not active
		if ( !active )
			inputValue.current = 0f;

	}

}
