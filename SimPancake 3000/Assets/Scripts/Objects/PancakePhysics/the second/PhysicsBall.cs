using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBall : MonoBehaviour
{

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

}
