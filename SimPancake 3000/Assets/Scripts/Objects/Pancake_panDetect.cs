using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pancake_panDetect : MonoBehaviour
{

	[SerializeField] private RaycastHit[] hits = new RaycastHit[ 1 ];
	[SerializeField] private LayerMask hitLayers;
	[SerializeField] private float rayDistance = 1;
	[Tooltip("If null uses self")]
	[SerializeField] private Transform directionTransform;
	[SerializeField] private Vector3 direction = new Vector3(0, 0, 1);

	// <0 == no hit.
	public float HitDistance {
		get;
		private set;
	} = -1;

	private void Start()
	{
		if ( directionTransform == null )
			directionTransform = transform;
	}

	void FixedUpdate()
    {
		Vector3 direct = directionTransform.TransformDirection(direction);
		// should forwards be the pan??
		Ray ray = new Ray(transform.position, direct);
		
		// look for the edge of the pan.
		int hitCount = Physics.RaycastNonAlloc(ray, hits, rayDistance, hitLayers);

		if ( hitCount > 0 )
			HitDistance = hits[ 0 ].distance;
		else
			HitDistance = -1;

		Debug.DrawRay(ray.origin, ray.direction, Color.red);

    }

	/// <summary>
	/// Gets the hit location in world space
	/// </summary>
	/// <returns>null if no hi else returns the hit location in world space</returns>
	public Vector3 GetHiyLocation()
	{

		if ( HitDistance < 0 ) return Vector3.zero;
		else return hits[ 0 ].point;

	}

	public void SetDirectionTransform(Transform trans)
	{
		directionTransform = trans;
	}

}
