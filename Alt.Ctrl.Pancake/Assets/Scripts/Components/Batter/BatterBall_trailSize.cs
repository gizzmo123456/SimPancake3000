using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Batter_quantity)), RequireComponent(typeof(TrailRenderer))]
public class BatterBall_trailSize : MonoBehaviour, IBatterChanged
{

	private Batter_quantity batterBallQuantity;
	private TrailRenderer trailRenderer;

	[SerializeField] private float widthMutilplier = 1.5f;

    void Awake()
    {

		batterBallQuantity = GetComponent<Batter_quantity>();
		trailRenderer = GetComponent<TrailRenderer>();

		batterBallQuantity.OnBatterChanged += OnBatterChanged;

    }

	public void OnBatterChanged( float batterprecent )
	{

		trailRenderer.widthMultiplier = batterprecent * widthMutilplier;

	}
}
