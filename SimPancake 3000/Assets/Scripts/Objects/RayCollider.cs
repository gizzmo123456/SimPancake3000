using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RayCollider : MonoBehaviour
{

	private Rigidbody rigid;

	public delegate void rayHit(RaycastHit hit);
	public event rayHit RayHit;

	private RaycastHit[] rayHits = new RaycastHit[1];
	[SerializeField] private LayerMask hitLayers;
	[SerializeField] private Vector3 direction = new Vector3(0, -1, 0);

	[Range( 0f, 1f )]
	[SerializeField] private float distanceCorrectionMulitplyer;

	public bool debug = false;

    // Start is called before the first frame update
    private void Awake()
    {
		rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

		float distance = ( ( transform.position.y + ( rigid.velocity.y * Time.deltaTime ) ) - transform.position.y ) + ( transform.lossyScale.y / 2f );

		int hitCount = CastRay( transform.position, direction, distance + 2f );

		if ( Physics.Raycast(transform.position, direction, out rayHits[0], distance + ( distance * distanceCorrectionMulitplyer ), hitLayers ) ) // hitCount > 0)
		//if ( Physics.SphereCast(transform.position,0.5f, direction, out rayHits[0], distance + ( distance * distanceCorrectionMulitplyer ), hitLayers ) ) // hitCount > 0)
		//if ( Physics.BoxCast(transform.position, Vector3.one/2f, direction, out rayHits[0], Quaternion.identity, distance + ( distance * distanceCorrectionMulitplyer ), hitLayers ) ) // hitCount > 0)
		{
			if(debug)
				print( "Rigid: "+rigid.velocity +" name: "+ rayHits[0].collider.name + " hitCount: "+hitCount);

			Vector3 pos = rayHits[ 0 ].point;
			pos.y += transform.lossyScale.y / 2f;

			transform.position = pos;

			Vector3 vel = rigid.velocity;

			if (rigid.useGravity)
				vel.y = -Physics.gravity.y * Time.deltaTime;	//counter unitys velocity for the next update :)

			rigid.velocity = vel;

			RayHit?.Invoke( rayHits[ 0 ] );
			
		}
		else if( !rigid.useGravity )
		{

		}

		Debug.DrawRay(transform.position, direction * (distance + ( distance * distanceCorrectionMulitplyer ) ), Color.red);


#if UNITY_EDITOR
		DrawHit();
#endif

	}

	protected virtual int CastRay(Vector3 orgin, Vector3 direct, float distance)
	{

		return Physics.RaycastNonAlloc(orgin, direct, rayHits, distance, hitLayers);

	}

#if UNITY_EDITOR
	private void DrawHit()
	{

	}
#endif

}
