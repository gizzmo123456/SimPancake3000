using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCtrl : MonoBehaviour
{

	private Rigidbody rigid;
	[SerializeField] private ForceCtrl master;
	[Header("--")]
	[SerializeField] private ForceCtrl side_a;
	[SerializeField] private ForceCtrl side_b;

	[SerializeField] private ForceCtrl inner;   // leave black if master/center ?

	private Vector3 velocity = Vector3.zero;
	private Vector3 last_velocity = Vector3.zero;
	[SerializeField] float forceMutiplyer = 1f;

	ForceCtrl fromCntr;
	bool sent = false;

    // Start is called before the first frame update
    void Start()
    {
		rigid = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
		sent = false;
		Vector3 vel = velocity;

		if ( velocity.y <= 0 )
			vel.y = rigid.velocity.y;

		rigid.velocity = velocity;

		SendVelocity();

		last_velocity = velocity;
		velocity.y -= 9.82f * Time.deltaTime;

		if ( velocity.y < 0 ) velocity.y = 0;


    }

	public void AddForceToPoint(Vector3 forceToApply)
	{

		velocity += forceToApply;

	}

	public void AddForceToPoint( Vector3 forceToApply, ForceCtrl from )
	{
		if ( sent ) return;

		velocity += forceToApply;
		fromCntr = from;
	}

	public void SendVelocity()
	{
		sent = true;
		if ( side_a != null && fromCntr != side_a)
			side_a.AddForceToPoint( last_velocity );

		if ( side_b != null && fromCntr != side_b )
			side_b.AddForceToPoint( last_velocity );

		if ( inner != null && fromCntr != inner )
			inner.AddForceToPoint( last_velocity );
	}

}
