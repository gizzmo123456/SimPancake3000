using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tiltBall : MonoBehaviour
{
	[SerializeField] private int panId = 0;
	[SerializeField] private Transform pan;
	[SerializeField] private float range = 2; 

	// Update is called once per frame
	void Update()
    {

		InputValues inputs = InputHandler.GetInputs();

		float x_pos = /*pan.position.x + */(( inputs.pans_x[ panId ] / 180f ) * range);
		float z_pos = /*pan.position.z + */(( inputs.pans_y[ panId ] / 180f ) * range);

		transform.localPosition = new Vector3( z_pos, x_pos, 0 );

    }  
}
