using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

public class Jug : MonoBehaviour
{

	[Header("Inputs and limits")]
	[SerializeField] private MinMax minMaxInputValue = new MinMax( -10, 90 );
	[SerializeField] private MinMax minMaxRotation = new MinMax( -10, 90 );

	[Header("Batter")]
	[SerializeField] private Transform batter;
	private float batter_amount = 1f;
	[SerializeField] private float batter_maxPourRate = 0.2f;   // per second
	[SerializeField] private MinMax batter_yScale_pour = new MinMax(0.2f, 1f);

	[Header("Position")]
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

		minMaxInputValue.current = inputs.jug;
		rotation.x = minMaxRotation.GetValue( minMaxInputValue.Precent );

		print( minMaxInputValue.Precent );
		transform.eulerAngles = rotation;

		// keep the batters x rotation @ 0 so its always level.
		// TODO: make dynamic.
		Vector3 batterRotation = batter.eulerAngles;
		Vector3 batterScale = batter.localScale;

		batterRotation.x = 0;
		batter.eulerAngles = batterRotation;

		batterScale.y = batter_yScale_pour.GetValue( minMaxInputValue.Precent );

		batter.localScale = batterScale;

	}



}
