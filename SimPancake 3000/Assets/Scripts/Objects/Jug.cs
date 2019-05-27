using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Jug : MonoBehaviour
{

	[SerializeField] private Transform batter;

	[SerializeField] private MinMax MinMaxRotation = new MinMax(-10, 90);
	[SerializeField] private MinMax MinMaxInputValue = new MinMax(-10, 90);

	[SerializeField] private Vector3 defaultPosition = Vector3.zero;
	[SerializeField] private Transform[] fryingPans;
	[SerializeField] private Vector3 fryingPanOffset;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		InputValues inputs = InputHandler.GetInputs();

		Vector3 rotation = transform.eulerAngles;

		rotation.x = inputs.jug;
		print( inputs.jug );
		transform.eulerAngles = rotation;

		// keep the batters x rotation @ 0 so its always level.
		// TODO: make dynamic.
		Vector3 batterRotation = batter.eulerAngles;

		batterRotation.x = 0;
		batter.eulerAngles = batterRotation;

	}



}
