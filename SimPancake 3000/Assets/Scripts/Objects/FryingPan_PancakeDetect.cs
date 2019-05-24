using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan_PancakeDetect : MonoBehaviour
{

    [SerializeField]
    private FryingPan fryingPan;

    private void OnTriggerEnter( Collider other )
    {
        
        // reg the pancake into the frying pan.
        if(fryingPan && other.CompareTag("pancake"))
        {
            fryingPan.RegisterPancake( other.GetComponent<Pancake>() );
        }

    }

    private void OnTriggerExit( Collider other )
    {

        // reg the pancake into the frying pan.
        if ( fryingPan && other.CompareTag( "pancake" ) )
        {
            fryingPan.UnregisterPancake( other.GetComponent<Pancake>() );
        }

    }

}
