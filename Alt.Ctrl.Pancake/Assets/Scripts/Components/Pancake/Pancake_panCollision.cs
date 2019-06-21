using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_panCollision : MonoBehaviour, IPanCollider
{

	private Transform panColliderObj;

	public void SetPanCollider( Transform panColl )
	{
		panColliderObj = panColl;
	}

}
