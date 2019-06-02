using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof(Rigidbody))]
public class Pancake : MonoBehaviour
{

    private Rigidbody rb;
    private FryingPan currentPan;   //null if its not in a pan

	private Vector3 startPosition;
	private PancakeState currentPancakeState = PancakeState.Mixture;

	private Vector3 startLerpScale = Vector3.zero;
	private Vector3 targetScale = Vector3.zero;
	private float targetLerpLength = 0.5f;
	private float targetLerpTime = 0f;

	private float volume = 0;
	private float radius = 0.01f;
	[SerializeField] private float maxRadius = 1f;
	[SerializeField] private float minHeight = 0.1f;
	private float Height
	{
		get{ return volume / ( Mathf.PI * Mathf.Pow( radius, 2 ) ); }
	}
/*
	[Range( 0.1f, 1.5f )]
	[SerializeField] private float defaultSpreadRate = 0.25f;   //Per Second
	[SerializeField] private MinMax pancakeHeight_spreadRate = new MinMax( 0.1f, 0.25f );
	[SerializeField] private Vector3 maxLocalScale = Vector3.one;	//Local to the frying pan.
*/
	private void Awake()
	{
		transform.localScale = Vector3.zero;	//New pancakes should have a size of zero :).	
	}

	void Start()
    {
        rb = GetComponent<Rigidbody>();
		currentPancakeState = PancakeState.Mixture;
	}

    void Update()
    {
        WakeUp();

		if( currentPancakeState == PancakeState.Mixture )
		{
			transform.position = startPosition + new Vector3(0, 0.01f, 0);// new Vector3(0, 0.01f + ((transform.localScale.y/2f) * transform.parent.localScale.y) , 0);
			transform.eulerAngles = new Vector3( 0, 90, 0 );
			SpreadPancakeBatter();
			LerpPancakeSize();
		}

    }

	private void SpreadPancakeBatter() 
	{
		
		if(targetScale.y > minHeight)
			radius += 0.1f * Height * Time.deltaTime;
		
		if ( radius > maxRadius ) radius = maxRadius;

		targetScale.y = Height > minHeight ? Height : minHeight;
		targetScale.x = radius;// / 2f;
		targetScale.z = radius;// / 2f;

	}

	private void LerpPancakeSize()
	{
		targetLerpTime += Time.deltaTime;

		if ( targetLerpTime > targetLerpLength )
			targetLerpTime = targetLerpLength;

		transform.localScale = Vector3.Lerp( startLerpScale, targetScale, ( targetLerpTime / targetLerpLength ) );

	}

    public void SetCurrentPan(FryingPan fryingPan)
    {
        currentPan = fryingPan;
    }

    public void WakeUp()
    {
        if (rb.IsSleeping())
        {
            print("Stop sleeping on the job!");
            rb.WakeUp();
        }
    }

	public void AddForce(float force)
	{
		rb.AddForce( new Vector3( 0, force, 0 ) );
		//rb.AddTorque(new Vector3(-1500f * force, 0, 0) );
	}

	public void AddBatter( float batterQt, float intervalLength )
	{

		//check we are in a raw state.
		if( currentPancakeState != PancakeState.Mixture )
		{
			Debug.LogError( "Can only add batter to a raw pancake." );
			return;
		}

		startLerpScale = transform.localScale;
		volume += batterQt;
		targetLerpLength = intervalLength;
		targetLerpTime = 0;

	}

	public void SetStartPosition(Vector3 worldStartPos)
	{
		startPosition = worldStartPos;
	}

	public PancakeState GetCurrentState()
	{
		return currentPancakeState;
	}
}

public enum PancakeState	//Use State?
{
	Mixture,
	Raw,
	Cooked,
	Burnt,
	Fire
}
