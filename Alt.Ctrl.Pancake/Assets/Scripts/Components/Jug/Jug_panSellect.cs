using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a bit of a temp till i get round to the webcam bit :)
public class Jug_panSellect : BasePanGroup_singleInput
{

	public delegate void onPanChanged( int id );
	public event onPanChanged OnPanChanged;

	private int currentPan = -1;    //-1 = default position.
	[Header("Pan Select")]
	[SerializeField] private Vector3 defaultPosition = Vector3.zero;
	[SerializeField] private Transform[] panTransforms; // should be in order of id's
	[SerializeField] private Vector3 panOffset;

	[SerializeField] private float releaseThresshold = 0.1f; // secs
	private float releaseTimer = 0f;

	public bool isPressed = false;

	protected override void Update()
    {

		base.Update();

		if ( ( !isPressed && inputValue.ClampedPrecent <= 0.5f ) || ( isPressed && inputValue.ClampedPrecent < 0.5f && releaseTimer > 0f ) )
		{
			isPressed = true;
			releaseTimer = 0;
			NextPan();
		}
		else if ( isPressed && inputValue.ClampedPrecent > 0.5f )
		{
			if ( releaseTimer < releaseThresshold )
			{
				releaseTimer += Time.deltaTime;
			}
			else
			{
				isPressed = false;
				NextPan( true );

			}
		}


    }


	private void NextPan( bool reset = false)
	{

		currentPan = reset ? -1 : ++currentPan;

		if ( currentPan >= panTransforms.Length )
			currentPan = -1;
		
		Vector3 position = defaultPosition;

		if ( currentPan != -1 )
			position = panTransforms[ currentPan ].position + panOffset;

		transform.position = position;

		OnPanChanged?.Invoke( currentPan );

	}

	public int GetCurrentPanId()
	{
		return currentPan;
	}
}
