using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactNormal : MonoBehaviour
{
	private void OnCollisionStay( Collision collision )
	{
		
		if(collision.contactCount > 0)
		{

			print( "contacts: " + collision.contactCount + " Norm: " + collision.contacts[ 0 ].normal );

		}

	}
}
