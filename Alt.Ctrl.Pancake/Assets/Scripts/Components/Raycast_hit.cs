using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Raycast_hit : MonoBehaviour
{

	public delegate void onRayHit( Vector3 hitLocation, Collider collider );
	public event onRayHit OnRayHit;

	[SerializeField] private RaycastHit rayHit;
	[SerializeField] private LayerMask hitLayers;

	[SerializeField] private Vector3 direction = Vector3.up;
	[SerializeField] private bool useLocalDirection;

	[SerializeField] protected float distance = 1f;

	[Header("Debug")]
	[SerializeField] private bool debug = false;
	[SerializeField] private Color debug_lineColor = Color.red;



	protected virtual void FixedUpdate()
    {

		if ( CastRay() )
			OnRayHit?.Invoke( rayHit.point, rayHit.collider );

    }

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
