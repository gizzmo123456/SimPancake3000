using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tiltBall : MonoBehaviour
{
	[SerializeField] private int panId = 0;
	[Tooltip("this is the transform for just the pan, excluding the handled ect...")]
	[SerializeField] private Transform pan;
	[Tooltip( "this is the ref to the frying pan object it self." )]
	[SerializeField] private FryingPan fryingPan;
	[SerializeField] private float range = 2;
	[Tooltip("The min range from the center that will affet the batter")]
	[SerializeField] private float minRange = 0.1f;

	public bool debug = false;

	// Update is called once per frame
	void Update()
    {

		InputValues inputs = InputHandler.GetInputs();

		float x_pos = /*pan.position.x + */(( inputs.pans_x[ panId ] / 180f ) * range);
		float z_pos = /*pan.position.z + */(( inputs.pans_y[ panId ] / 180f ) * range);

		transform.localPosition = new Vector3( z_pos, x_pos, 0 );

		// only find the pancakes cloest vert points if the we have titled enought to afect the batter
		//if ( Vector3.Distance( Vector3.zero, transform.localPosition ) < minRange ) return;

		if( debug )
			print("In Range od manip");

		//TODO: check if the pancake is in a batter/mixture state
		Pancake pancake = fryingPan.GetCurrentPancakes();

		if( pancake != null)
		{
			PancakeMeshControler pancakeMesh = pancake.GetComponent<PancakeMeshControler>();

			if ( pancakeMesh != null )
			{
				ManipulatePancakeVert(pancake.transform, pancakeMesh);
			}

		}
		

    }

	void ManipulatePancakeVert( Transform pancake, PancakeMeshControler pancakeMesh )
	{
		// find the cloest physics ball that belogs to the pancake.

		PancakeMeshControler.VerticeGroup[] pancakeVerts = pancakeMesh.GetVertGroups();
		float cloestDist = 0;
		int cloestId = -1;

		for ( int i = 0; i < pancakeVerts.Length; i++)
		{
			float distanceToVert = Vector3.Distance( transform.position, pancakeVerts[ i ].physicsBall.transform.position );

			if (cloestId == -1 || distanceToVert < cloestDist)
			{
				cloestDist = distanceToVert;
				cloestId = i;
			}

		}

	//	pancakeMesh.UpdateVertPosition( cloestId, transform.localPosition.x, transform.localPosition.z );

		if (debug)
			print("#### "+cloestId+" ## Dist: "+cloestDist + " ## lp: "+transform.localPosition );
	}

}
