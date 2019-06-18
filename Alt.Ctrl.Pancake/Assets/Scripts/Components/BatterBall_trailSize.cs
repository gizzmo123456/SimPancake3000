using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Batter_quantity)), RequireComponent(typeof(TrailRenderer))]
public class BatterBall_trailSize : MonoBehaviour
{

	private Batter_quantity batterBallQuantity;
	private TrailRenderer trailRenderer;

	[SerializeField] private float widthMutilplier = 1.5f;

    void Awake()
    {

		batterBallQuantity = GetComponent<Batter_quantity>();
		trailRenderer = GetComponent<TrailRenderer>();

		batterBallQuantity.OnBatterAdded += OnBatterAdded;

    }

	private void OnBatterAdded( float amountAdded )
	{

		trailRenderer.widthMultiplier = batterBallQuantity.GetBatterPrecent() * widthMutilplier;

	}
}
