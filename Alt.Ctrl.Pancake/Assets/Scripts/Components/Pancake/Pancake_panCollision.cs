using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_panCollision : MonoBehaviour, IPanCollider
{

	private Transform panColliderObj;
	/*private*/public Vector3 positionInPan;          // the local position of the pancake in the pan

	private void Update()
	{

		if ( panColliderObj == null ) return;

		transform.eulerAngles = panColliderObj.eulerAngles;
		transform.position = panColliderObj.TransformPoint(positionInPan);

	}

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;

		if ( panColl != null )
			positionInPan = panColl.InverseTransformPoint( transform.position );
	}

}
