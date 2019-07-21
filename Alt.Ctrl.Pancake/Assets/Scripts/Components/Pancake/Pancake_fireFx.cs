using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Pancake_fireFx : MonoBehaviour
{
	[Tooltip(" Local scale so it is re scaled to its parent object, aka the pancake :D ")]
	[SerializeField] private Vector3 localScale = Vector3.one;

	[Header(" Fire Fx ")]
	[SerializeField] private ParticleSystem fire;
	[SerializeField] private Vector3 fire_minShapeScale = new Vector3( 0.1f,  0.1f, 0.1f );
	[SerializeField] private Vector3 fire_maxShapeScale = new Vector3( 0.75f, 0.1f, 0.75f );
	[SerializeField] private MinMax fire_emissionRateRange = new MinMax( 75, 250 );
	[SerializeField] private MinMax fire_velocityOffsetYRange = new MinMax( 0, 5 );
	[SerializeField] private MinMax fire_velocitySpeedModRange = new MinMax( 0, 0.6f );
	[SerializeField] private MinMax fire_SimulationSpeedRange = new MinMax( 0, 0.4f );

	[SerializeField] private Light fireLight;
	[SerializeField] private MinMax fire_lightIntensityRange = new MinMax(0f, 10f);

	[ Header( " Smoke Fx " )]
	[SerializeField] private ParticleSystem smoke;
	[SerializeField] private Vector3 smoke_minShapeScale = new Vector3( 0.1f, 0.1f, 0.1f );
	[SerializeField] private Vector3 smoke_maxShapeScale = new Vector3( 0.75f, 0.1f, 0.75f );
	[SerializeField] private MinMax smoke_emissionRateRange = new MinMax( 75, 250 );
	[SerializeField] private MinMax smoke_alphaRange = new MinMax( 0, 1 );


	[Header( " Shared " )]
	[SerializeField] private float warmup_length = 5; //sec
	private float warmup_timer = 0;

	private void Start()
	{

		transform.localScale = localScale;

	}

	// Update is called once per frame
	void Update()
    {

		warmup_timer += Time.deltaTime;
		float warmup_percentage = Mathf.Clamp01(warmup_timer / warmup_length);

		// fire
		ParticleSystem.ShapeModule fire_shapeMod = fire.shape;
		ParticleSystem.EmissionModule fire_emissionMod = fire.emission;
		ParticleSystem.VelocityOverLifetimeModule fire_velocityMod = fire.velocityOverLifetime;
		ParticleSystem.MainModule fire_mainMod = fire.main;

		fire_shapeMod.scale = Vector3.Lerp( fire_minShapeScale, fire_maxShapeScale, warmup_percentage );
		fire_emissionMod.rateOverTime = fire_emissionRateRange.GetValue( warmup_percentage );
		fire_mainMod.simulationSpeed = fire_SimulationSpeedRange.GetValue( warmup_percentage );
		fire_velocityMod.orbitalOffsetY = fire_velocityOffsetYRange.GetValue( warmup_percentage );
		fire_velocityMod.speedModifierMultiplier = fire_velocitySpeedModRange.GetValue( warmup_percentage );

		fireLight.intensity = fire_lightIntensityRange.GetValue( warmup_percentage );

		// smoke
		ParticleSystem.ShapeModule smoke_shapeMod = smoke.shape;
		ParticleSystem.EmissionModule smoke_emissionMod = smoke.emission;
		ParticleSystem.MainModule smoke_mainMod = smoke.main;

		smoke_shapeMod.scale = Vector3.Lerp( smoke_minShapeScale, smoke_maxShapeScale, warmup_percentage );
		smoke_emissionMod.rateOverTime = smoke_emissionRateRange.GetValue( warmup_percentage );

		Color currentColor = smoke_mainMod.startColor.color;
		currentColor.a = smoke_alphaRange.GetValue( warmup_percentage );
		smoke_mainMod.startColor = currentColor;

	}

}
