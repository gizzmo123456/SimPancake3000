using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_outOfBounds : MonoBehaviour
{

	[SerializeField] private float minY_outOfRange = -10;

    void Update()
    {

		if ( transform.position.y < minY_outOfRange )
			Destroy( gameObject );

    }

}
