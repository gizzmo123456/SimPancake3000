using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BatterTrail : MonoBehaviour
{
	[SerializeField] private Jug jug;
	[SerializeField] private float maxLifeTime = 1.5f;
	private float spwanTime = 0;

	[Header( "Batter Pour Trail" )]
	private TrailRenderer trailRenderer;
	[SerializeField] private Transform pourTrail_startPosition;
	[SerializeField] private Transform pourTrail_lerpEndPosition;
	[SerializeField] private float pourTrail_rotationThresshold = 10;
	[SerializeField] private float pourTrail_rotationLerpRange = 30;

	private float batterQuantity = 0;
	private bool batterPoured = false;

	[Header( "Batter Colour" )]
	[SerializeField] private Color batterColor_a;
	[SerializeField] private Color batterColor_b;

	// Start is called before the first frame update
	private void Start()
    {
		trailRenderer = GetComponent<TrailRenderer>();
		spwanTime = Time.time;

		Color col;

		col = Color.Lerp( batterColor_a, batterColor_b, Random.value );
		trailRenderer.material.SetColor( "_Color", col );

		col = Color.Lerp( batterColor_a, batterColor_b, Random.value );
		trailRenderer.material.SetColor( "_EmissionColor", col );
    }

	public void Init( Jug j, Transform startLerpPosition, Transform endLerpPosition, float batterQt )
	{
		jug = j;
		pourTrail_startPosition = startLerpPosition;
		pourTrail_lerpEndPosition = endLerpPosition;

		batterQuantity = batterQt;
		//TODO: change the size of the trail depending on the qt.

	}

	// Update is called once per frame
	private void Update()
    {


		if ( !batterPoured )// Mathf.Abs( jug.GetCurrentXRotation() ) > pourTrail_rotationThresshold && Mathf.Abs( jug.GetCurrentXRotation() ) < pourTrail_rotationThresshold + pourTrail_rotationLerpRange )
		{
			float lerpPercentage = ( Mathf.Abs( jug.GetCurrentXRotation() ) - pourTrail_rotationThresshold ) / pourTrail_rotationLerpRange;
			lerpPercentage = Mathf.Clamp01( lerpPercentage );

			transform.position = Vector3.Lerp( pourTrail_startPosition.position, pourTrail_lerpEndPosition.position, lerpPercentage );

			if ( lerpPercentage >= 0.9f )
				batterPoured = true;

		}
		else if(batterPoured && Mathf.Abs( jug.GetCurrentXRotation() ) < pourTrail_rotationThresshold )
		{
			trailRenderer.time -= Time.deltaTime;
		}

		if ( Time.time >= spwanTime + maxLifeTime )
			Destroy(gameObject);

	}

	// Tell the pan to start spwaning a pancake, if it hasent already :)
	private void OnCollisionEnter( Collision collision )
	{

		if( collision.gameObject.CompareTag("fryingPan") )
		{
			// start to creat our pancake in the frying pan
			collision.gameObject.GetComponentInParent<FryingPan>().BatterCollision( collision.contacts[0].point, batterQuantity );
		}

	}

}
