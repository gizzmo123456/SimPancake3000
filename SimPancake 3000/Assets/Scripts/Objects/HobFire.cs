using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof(ParticleSystem))]
public class HobFire : MonoBehaviour
{
	private ParticleSystem particleSystem;
	
    [SerializeField] private int hobID = 0;
	[SerializeField] private MinMax lifetime = new MinMax( 1f, 1.75f );
	[SerializeField] private MinMax yForce = new MinMax( 5f, 15f );
	[SerializeField] private MinMax emissions = new MinMax( 100f, 300f );


	[SerializeField] private FryingPan fryingPan;
	[SerializeField] private float minTemperture = 25f;
	[SerializeField] private float maxTemperture = 150f;

	void Awake()
    {
		particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        InputValues inputs = InputHandler.GetInputs();

        //TODO: check that the panID is in range of pan count once we have a Static Game Class
        if (hobID < 0 || hobID >= 3) //i know there 3 but still.
        {
            Debug.LogError("Pan Id Out of range (obj name: " + name + ")");
            return;
        }

		// Update the particleSystem 
		float hobPercent = ( inputs.hobs[ hobID ] / 1023f );

		ParticleSystem.MainModule mainMod = particleSystem.main;
		mainMod.startLifetime = lifetime.GetValue( hobPercent );

		ParticleSystem.ForceOverLifetimeModule forceMod = particleSystem.forceOverLifetime;
		forceMod.y = yForce.GetValue( hobPercent );

		ParticleSystem.EmissionModule emissionMod = particleSystem.emission;
		emissionMod.rateOverTime = emissions.GetValue( hobPercent );

		// send temp to pan.
		float temperureDif = maxTemperture - minTemperture;
		float temp = minTemperture + ( temperureDif * hobPercent );
		fryingPan.AddTempture( temp );

    }
}
