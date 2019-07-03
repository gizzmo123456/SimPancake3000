using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof( FryingPan_temperature ))]
public class FryingPan_steam : MonoBehaviour
{

	private FryingPan_temperature panTemperature;

	[SerializeField] private ParticleSystem steamParticleEffect;
	private ParticleSystem.EmissionModule particleEffectEmissions;
	private ParticleSystem.MainModule particleEffectMain;

	[SerializeField] private MinMax temperatureRange = new MinMax( 100, 300 );
	[SerializeField] private MinMax particleMultiplierRange = new MinMax( 0f, 1f );

	[SerializeField] private Gradient colorRange = new Gradient(); 




	private void Awake()
	{
		panTemperature = GetComponent<FryingPan_temperature>();
		particleEffectEmissions = steamParticleEffect.emission;
		particleEffectMain = steamParticleEffect.main;
	}

	private void Update()
	{

		if ( steamParticleEffect == null ) return;

		temperatureRange.current = panTemperature.GetCurrentTemprature();

		particleEffectEmissions.rateOverTimeMultiplier = particleMultiplierRange.GetValue( temperatureRange.ClampedPrecent );
		particleEffectMain.startColor = colorRange.Evaluate( temperatureRange.ClampedPrecent );

	}

}
