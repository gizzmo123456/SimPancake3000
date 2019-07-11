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

	public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
	{
		// loop x, y, z axis in vector.
		// [0] = x, [1] = y, [2] = z
		for (int v = 0; v < 3; v++ )
		{
			if ( value[ v ] < min[ v ] ) value[ v ] = min[ v ];
			if ( value[ v ] > max[ v ] ) value[ v ] = max[ v ];
		}

		return value;

	}

}
