using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Batter_quantity))]
public class Batter_scale : MonoBehaviour, IBatterChanged
{

	[SerializeField] private float maxYScale = 1f;
	[SerializeField] private Transform scaleObj;

	// Start is called before the first frame update
	void Start()
    {
		GetComponent<Batter_quantity>().OnBatterChanged += OnBatterChanged;	//Since we return the remaining batter back to the jug when we use 
    }

	public void OnBatterChanged( float batterPrecentage )
	{

		Vector3 scale = scaleObj.localScale;
		scale.y = maxYScale * batterPrecentage;
		scaleObj.localScale = scale;

	}

}
