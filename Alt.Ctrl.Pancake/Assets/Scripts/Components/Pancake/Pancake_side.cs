using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pancake_state))]
public class Pancake_side : MonoBehaviour, IPanCollider
{

	private Pancake_state state;


    void Start()
    {

		state = GetComponent<Pancake_state>();

    }

    void UpdateSideDown()
    {

		// work out what face of the pancake is in the frying pan.
		// work out the world position of the top and bottom face from it local space
		// to see which face is on top/bottom.

		Vector3 localTop = new Vector3( 0, 1, 0 );

		// get the top and bottom local position in world space.
		// only need y axis as we are finding which is highest
		float worldTop = transform.TransformPoint( localTop ).y;
		float worldBottom = transform.TransformPoint( -localTop ).y;

		if ( worldTop >= worldBottom )   // side 0, the local top of the pancake is not face down in the pan 
			state.SetSideDown( 0 );
		else                            // side 1, the local top of the pancake is face down in the pan.
			state.SetSideDown( 1 );



	}

	public void SetPanCollider( Transform panObj )
	{

		// if the pan obj is null we are leaving the frying pan so we dont need to do anythink.
		// else if it is not null we are entering the pan, so we need to update which side is face down in the pan.
		if ( panObj == null ) return;

		UpdateSideDown();

	}

}
