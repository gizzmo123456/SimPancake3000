using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosition : MonoBehaviour
{
	[SerializeField] private Vector3 localPosition = Vector3.zero;

	void Update()
    {
		transform.localPosition = localPosition;
    }
}
