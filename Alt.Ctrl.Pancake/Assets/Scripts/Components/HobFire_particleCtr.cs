using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class HobFire_particleCtr : BasePanGroup_singleInput
{
	[Header("HobFire")]
	[SerializeField] private ParticleSystem particleSys;

	[SerializeField] private MinMax lifetime = new MinMax( 1f, 1.75f );
	[SerializeField] private MinMax yForce = new MinMax( 5f, 15f );
	[SerializeField] private MinMax emissions = new MinMax( 100f, 300f );

    protected override void Update()
    {

		base.Update();

		UpdateParticleSystem();

	}

	private void UpdateParticleSystem()
	{
		ParticleSystem.MainModule mainMod = particleSys.main;
		mainMod.startLifetime = lifetime.GetValue( inputValue.Precent );

		ParticleSystem.ForceOverLifetimeModule forceMod = particleSys.forceOverLifetime;
		forceMod.y = yForce.GetValue( inputValue.Precent );

		ParticleSystem.EmissionModule emissionMod = particleSys.emission;
		emissionMod.rateOverTime = emissions.GetValue( inputValue.Precent );
	}
}
