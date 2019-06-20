using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BatterBall_timeCorrection_onRelease : MonoBehaviour, IBatterRelease
{

	private float spwanTime;
	private TrailRenderer trail;

	void Awake()
    {
		trail = GetComponent<TrailRenderer>();
		spwanTime = Time.time;
    }

	public void OnBatterRelease()
	{

		float timeDif = trail.time - (Time.time - spwanTime);

		if (timeDif > 0)
			trail.time -= timeDif;

	}

	public void SetTime( float length)
	{
		trail.time = length;

	}

}
