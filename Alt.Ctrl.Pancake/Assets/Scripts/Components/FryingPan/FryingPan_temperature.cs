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
	[Tooltip( " How much temprature can be conducted over maxTemp (so if it value at current temp is 1 it conducts all " 
			+ "temp, if value is 0 is conducts no temp), also reversed for cooldown " )]
	[SerializeField] private AnimationCurve panConductiveCurve;
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
		float knobInputVal = inputValues[ knob_inputId ].ClampedPrecent;					// pan knob value
		float hobInputVal = inputValues[ hob_inputId ].ClampedPrecent;						// pan on hob value
		float maxTemp = panTempertureCurve.Evaluate( knobInputVal ) * maxPanTemperature;	// max temp the pan can reach @ knob input value

		float currentTempPercentage = currentTemperature / maxPanTemperature;               // current % of very max pan temp (no limited to knob input value)
		float currentConductivity = panConductiveCurve.Evaluate(currentTempPercentage);     // how conductive is the pan the current temp
		float currentDissipation = 1f - currentConductivity;                                // how fast the heat can be dissipated from the pan (opersit of conductivity)

		if ( hobInputVal == 0 )                     // cool down if not on hob.
			currentTemperature -= cooldownRate * currentDissipation * Time.deltaTime;
		else if ( currentTemperature < maxTemp )    // heat up, not at max temp
			currentTemperature += ( minKnobValue + knobInputVal ) * hobInputVal * ( cookingTemperture * Time.deltaTime ) * currentConductivity * Time.deltaTime;
		else if ( currentTemperature > maxTemp )    // cool down over max temp
			currentTemperature -= ( currentTemperature - maxTemp ) * currentDissipation * Time.deltaTime;

		 if ( currentTemperature < 0 )				// we not in the antarctic
			currentTemperature = 0;

		// Right we need to do some shiz with this now :), send it to fryingpan pancake??, idk.
		// sounds logical, it knows what pancakes are in the pan.
		// :)
		// ...

		fryingPan_pancake.CookPancakes( currentTemperature );

	}

	public float GetCurrentTemprature()
	{
		return currentTemperature;
	}
}
