using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to be added when a pancake becomes a child of anouther pancake.
/// </summary>
public class Pancake_child : MonoBehaviour
{
	private Transform parent;
	//TODO: offset ect...

	public void SetParent( Transform par )
	{
		parent = par;

		// find all componentes on this object that are IChild's and notfi them that they are now a child of another pancake
		IChild[] children = GetComponents<IChild>();

		foreach ( IChild child in children )
			child.SetIsChild(true);

	}

    private void Update()
    {

		if ( parent == null ) return;

		// update the position and rotation of the pancake to its parent
		transform.position = parent.position;
		transform.eulerAngles = parent.eulerAngles;

    }
}
