using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RandomMaterialColor_OnStart : MonoBehaviour
{

	[SerializeField] private string propertyName = "_Color";
	[SerializeField] private Color colorA;
	[SerializeField] private Color colorB;

    void Start()
    {
		GetComponent<Renderer>().material.SetColor( propertyName, Color.Lerp( colorA, colorB, Random.value ) );
    }


}
