using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBall : MonoBehaviour
{
	// i am think that i might need to ref the cloest ball on the next ring in
	private PhysicsBall rightBall;
	private PhysicsBall leftBall;

	public void SetBalls(PhysicsBall ballA, PhysicsBall ballB)
	{

		if( IsLeft( ballA.transform.position ) )
		{
			leftBall = ballA;
			rightBall = ballB;
		}
		else
		{
			leftBall = ballB;
			rightBall = ballA;
		}


	}

	public bool IsLeft(Vector3 position)
	{
		return Mathf.Atan2( position.z - transform.position.z, position.y - transform.position.y ) < 0;
	}

	private void Update()
	{
		return;
		// make shore that the ball are always have a scale of 1, 1, 1 in worldSpace.
		Vector3 currentWorldScale = transform.lossyScale;

		if ( transform.parent == null || currentWorldScale.x < 0.05f || currentWorldScale.y < 0.05f || currentWorldScale.z < 0.5f ) return;

		Vector3 newScale = new Vector3(1f / currentWorldScale.x, 1f / currentWorldScale.y, 1f / currentWorldScale.z);
		transform.localScale = newScale;
		
	}

}
