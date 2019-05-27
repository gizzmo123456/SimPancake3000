using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jug : MonoBehaviour
{

	[SerializeField] private Vector3 defaultPosition = Vector3.zero;
	[SerializeField] private Transform[] fryingPans;
	[SerializeField] private Vector3 fryingPanOffset;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		InputValues inputs = InputHandler.GetInputs();

		Vector3 rotation = transform.eulerAngles;

		rotation.x = inputs.jug;

		transform.eulerAngles = rotation;

	}



}
