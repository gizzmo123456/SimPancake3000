using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BatterBall_timeCorrection_onRelease : MonoBehaviour
{

	float spwanTime;

    void Awake()
    {
		spwanTime = Time.time;
    }

	public void OnBatterRelease()
	{

		float timeDif = Time.time - spwanTime;

		GetComponent<TrailRenderer>().time -= timeDif;

	}

	public void SetTime( float length)
	{
		GetComponent<TrailRenderer>().time = length;

	}

}
