using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Raycast_hit : MonoBehaviour
{
/*	// On a reflection this seems a lil over enginered, insead i thin we should devive for this or the RB version.
	public delegate void onRayHit( Vector3 hitLocation, GameObject obj );
	public event onRayHit OnRayHit;
*/
	[SerializeField] protected RaycastHit rayHit;
	[SerializeField] private LayerMask hitLayers;

	[SerializeField] private Vector3 direction = Vector3.up;
	[SerializeField] private bool useLocalDirection;

	[SerializeField] protected float distance = 1f;

	[Header("Debug")]
	[SerializeField] private bool debug = false;
	[SerializeField] private Color debug_lineColor = Color.red;

	protected virtual bool CastRay()
	{

		bool hit = Physics.Raycast( transform.position, GetDirection().normalized, out rayHit, distance, hitLayers );

		if ( debug )
			Debug.DrawLine(transform.position, transform.position + (direction.normalized * distance), debug_lineColor );

		return hit;

	}

	protected Vector3 GetDirection()
	{
		return useLocalDirection ? transform.TransformDirection(direction) : direction;
	}

}
