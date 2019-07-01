using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FryingPan_pancake))]
public class FryingPan_temperature : BasePanGroup_multipleInput
{

	protected override int RequiredInputs { get { return 2; } }
	private FryingPan_pancake fryingPan_pancake;

	[SerializeField] private int hob_inputId = 0;
	[SerializeField] private int knob_inputId = 1;

	[Header("Temperature")]
	[Range( 0.1f, 0.75f )]
	[SerializeField] private float minKnobValue = 0.5f;  // hob value is minValue + 1
	[SerializeField] private float cookingTemperture = 250; //per second.

	[SerializeField] private float maxPanTemperature = 300;
	private float currentTemperature = 0;
	[Tooltip("pan max temperture curver for knob value")]
	[SerializeField] private AnimationCurve panTempertureCurve;
	[SerializeField] private float cooldownRate = 10f; //per second

	private void Start()
	{
		fryingPan_pancake = GetComponent<FryingPan_pancake>();
	}

	// Update is called once per frame
	protected override void Update()
    {

		base.Update();

		// Get our input values and update the current temp, if the pan is off the hob it will cool down
		float knobInputVal = inputValues[ knob_inputId ].ClampedPrecent;
		float hobInputVal = inputValues[ hob_inputId ].ClampedPrecent;
		float maxTemp = panTempertureCurve.Evaluate( knobInputVal ) * maxPanTemperature;


		if ( hobInputVal == 0 )	// cool down if not on hob.
			currentTemperature -= cooldownRate * Time.deltaTime;
		else if ( currentTemperature < maxTemp )
			currentTemperature += ( minKnobValue + knobInputVal ) * hobInputVal * cookingTemperture * Time.deltaTime;
		else if ( currentTemperature > maxTemp )
			currentTemperature -= ( currentTemperature - maxTemp ) * Time.deltaTime;

		 if ( currentTemperature < 0 )  // we not in the antarctic
			currentTemperature = 0;

		// Right we need to do some shiz with this now :), send it to fryingpan pancake??, idk.
		// sounds logical, it knows what pancakes are in the pan.
		// :)
		// ...

		fryingPan_pancake.CookPancakes( currentTemperature );

	}
}
