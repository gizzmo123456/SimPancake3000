﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_DEBUG : MonoBehaviour
{

	public GameObject pancake;
	public Transform panCollider;
	public Vector3 velocity;
	public bool setPanCollider = false;
	public bool updatePosition = false;
	public bool addVelocity = false;

    void Update()
    {

		if ( pancake == null ) return;

		if( setPanCollider )
		{
			pancake.SendMessage("SetPanCollider", panCollider);
			setPanCollider = false;
		}

		if ( updatePosition )
		{
			Pancake_panCollision panColl = pancake.GetComponent<Pancake_panCollision>();

			if( panColl.GetPanCollider() != null )
				panColl.positionInPan = panColl.GetPanCollider().InverseTransformPoint(transform.position);
		}

		if( addVelocity )
		{
			pancake.SendMessage( "AddVelocity", velocity );
			addVelocity = false;
		}

    }
}