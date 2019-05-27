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

	private bool batterPoured = false;

	// Start is called before the first frame update
	private void Start()
    {
		trailRenderer = GetComponent<TrailRenderer>();
		spwanTime = Time.time;
    }

	public void Init( Jug j, Transform startLerpPosition, Transform endLerpPosition )
	{
		jug = j;
		pourTrail_startPosition = startLerpPosition;
		pourTrail_lerpEndPosition = endLerpPosition;
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

			print( "!Boo ## "+lerpPercentage );
		}
		else if(batterPoured && Mathf.Abs( jug.GetCurrentXRotation() ) < pourTrail_rotationThresshold )
		{
			trailRenderer.time -= Time.deltaTime;
		}

		if ( Time.time >= spwanTime + maxLifeTime )
			Destroy(gameObject);

	}
}
