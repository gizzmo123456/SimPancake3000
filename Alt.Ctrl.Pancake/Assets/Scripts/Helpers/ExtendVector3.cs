using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendVector3
{
	public static Vector3 Multiply(this Vector3 vector, Vector3 multiValue)
	{
		Vector3 muli = vector;
		muli.x *= multiValue.x;
		muli.y *= multiValue.y;
		muli.z *= multiValue.z;

		return muli;
	}
}
