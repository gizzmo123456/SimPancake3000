using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* NOTES:
 * So using a method like this, causes the z axis to become the maintain distance form the pivit.
 * and the X and Y are offsets (or 0, 0 to rotate directly around the pivit point)
 * 
 */
 [RequireComponent(typeof(Rigidbody))]
public class RotateAround : MonoBehaviour
{

    public Transform pivitPoint;
    public Vector3 offset = Vector3.one;
    private float targetDistance;

    public float rotateAmount = 0;

    public Vector3 location = Vector3.one;

    public float rotateSpeed = 1f;
    private float currentRotateSpeed = 0;

    private Vector3 targetPosition = Vector3.zero;

    private Rigidbody rb;

    private Vector3 lastVelocity;
    private Vector3 externalVelocity;
    [SerializeField]
    private float friction = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

        //transform.LookAt( worldPoint );
        location = transform.position - pivitPoint.position;
        rb = GetComponent<Rigidbody>();
        targetDistance = transform.position.z;
    }

    private void FixedUpdate_()
    {

        // some more anit-gravity shiz :(
        Vector3 gravity = Physics.gravity;
        Vector3 velocity = Vector3.zero;// rb.velocity;

        Vector3 disDif = transform.position - targetPosition;

        //Cancel out the last bit of gravity
        //if(velocity.y > 0)
        velocity.y += -gravity.y * Time.deltaTime;

        // apply the perfect amount of force to hold it at target position;
        velocity.y = ( -disDif.y ) ;// * Time.deltaTime );

        //velocity.x = ( -disDif.x );// * Time.deltaTime );
        //velocity.z = ( -disDif.z );// * Time.deltaTime );
        externalVelocity.x += ( -disDif.x * Time.deltaTime );
        externalVelocity.z += ( -disDif.z * Time.deltaTime );
        //velocity.y += -gravity.y * Time.deltaTime; // Whay was this here ??
        

        // find how much extra force to add the the x n y axis.
        float totalDistDif = Mathf.Abs( disDif.x ) + Mathf.Abs( disDif.z );
        float yVelDif = velocity.y - lastVelocity.y;

        if ( yVelDif > 0 && totalDistDif > 0)
        {
            externalVelocity.x += ( Mathf.Abs( disDif.x ) / totalDistDif ) * yVelDif * ( disDif.x < 0 ? 1 : -1 );
            externalVelocity.z += ( Mathf.Abs( disDif.z ) / totalDistDif ) * yVelDif * ( disDif.z < 0 ? 1 : -1 );
        }
       /* else if (totalDistDif > 0)
        {
            externalVelocity.x -= ( Mathf.Abs( disDif.x ) / totalDistDif ) * yVelDif * ( disDif.x < 0 ? 1 : -1 );
            externalVelocity.z -= ( Mathf.Abs( disDif.z ) / totalDistDif ) * yVelDif * ( disDif.z < 0 ? 1 : -1 );
        }
        */
        // print( velocity +" "+ ( -disDif.y * Time.deltaTime ) );

        rb.velocity = velocity + externalVelocity;
        lastVelocity = velocity;

        UpdateExternalVelocity();
     //   print( "DIST ##"+Vector3.Distance( transform.position, pivitPoint.position ) +" ## Target: "+targetDistance +" ## %: "+( Vector3.Distance( transform.position, pivitPoint.position ) / targetDistance) );
    }

    private void FixedUpdate()
    {

        Vector3 gravity = Physics.gravity;
        Vector3 velocity = Vector3.zero;// rb.velocity;
        Vector3 disDif = transform.position - targetPosition;

        float downForce = -gravity.y;// * Time.deltaTime;

        float zForceAmount = Mathf.Atan2( pivitPoint.position.z - transform.position.z, pivitPoint.position.y - transform.position.y );// / ( Mathf.PI / 2f );
        float currentDistance = Vector3.Distance( transform.position, pivitPoint.position );
     //   print("ZForce: "+zForceAmount);

        Vector3 force = Vector3.zero;

        force.x = 0;
        force.y = 0;
        force.z = 0;

        ////////////
        float f = ( 1f - ( currentDistance / targetDistance ) );

     //   if ( f < 0 ) f -= 1f;

        float sin = Mathf.Sin( zForceAmount );// * Time.deltaTime );
        float cos = Mathf.Cos( zForceAmount );// * Time.deltaTime );

        float y = downForce;// 1f;// downForce;
        float z = 0;

        Vector3 newTargetPosition = Vector3.zero;
        Vector3 distanceCorrection = Vector3.zero;
        Vector3 correctedTargetPosition = Vector3.zero;

        newTargetPosition.y = y * cos - z * sin;
        newTargetPosition.z = z * cos + y * sin; 

        // correct the axis for the velocity
        correctedTargetPosition.y = newTargetPosition.z;

        if ( transform.position.z < pivitPoint.position.z )
            newTargetPosition = -newTargetPosition;

        correctedTargetPosition.z = -newTargetPosition.y;

        correctedTargetPosition.z = transform.position.y > pivitPoint.position.y ? -correctedTargetPosition.z : correctedTargetPosition.z;

        distanceCorrection = new Vector3(0, correctedTargetPosition.z, -correctedTargetPosition.y) * f;

        distanceCorrection.y = currentDistance > targetDistance ? Mathf.Abs( distanceCorrection.y ) : -Mathf.Abs( distanceCorrection.y );
        distanceCorrection.y = transform.position.y > pivitPoint.position.y ? -distanceCorrection.y : distanceCorrection.y;

        if ( transform.position.y > pivitPoint.position.y ) distanceCorrection.y = 0;

        externalVelocity.z += correctedTargetPosition.z;
        //externalVelocity.y += correctedTargetPosition.y ;

        //externalVelocity += distanceCorrection;

        print( "NP: "+newTargetPosition+"NNP: " + correctedTargetPosition +" # DF: "+downForce +" # targ%: " + f +" # distanceCorrection: "+distanceCorrection);
        rb.velocity = /*new Vector3(0, newnewPosition.y, 0) +*/ correctedTargetPosition + distanceCorrection + externalVelocity;// externalVelocity;// + distanceCorrection; // newnewPosition;

        UpdateExternalVelocity();


    }

    // Update is called once per frame
    void Update()
    {
        Rotate();

        currentRotateSpeed += ( ( rotateAmount / 90f ) - 1f ) * rotateSpeed;
        rotateAmount += -(currentRotateSpeed * Time.deltaTime);
    }

    void UpdateExternalVelocity()
    {

        if ( externalVelocity.x > 0 )
            externalVelocity.x -= friction * Time.deltaTime;
        else if ( externalVelocity.x < 0 )
            externalVelocity.x += friction * Time.deltaTime;

        if ( externalVelocity.y > 0)
            externalVelocity.y -= -Physics.gravity.y * Time.deltaTime;
        else if ( externalVelocity.y < 0 )
            externalVelocity.y += -Physics.gravity.y * Time.deltaTime;

        if ( externalVelocity.z > 0 )
            externalVelocity.z -= friction * Time.deltaTime;
        else if ( externalVelocity.z < 0 )
            externalVelocity.z += friction * Time.deltaTime;



    }

    void Rotate()
    {

        // deg to rad
        float rot = Mathf.Deg2Rad * rotateAmount;

        float sin = Mathf.Sin( rot );// * Time.deltaTime );
        float cos = Mathf.Cos( rot );// * Time.deltaTime );

        float y = location.y;
        float z = location.z;

        Vector3 newPosition = location;

        newPosition.y = y * cos - z * sin;
        newPosition.z = z * cos + y * sin;

        //location = newPosition;
        newPosition += pivitPoint.transform.position; 
        Vector3 newnewPosition = Vector3.zero;

        newnewPosition += newPosition.x * pivitPoint.transform.right;
        newnewPosition += newPosition.y * pivitPoint.transform.up;
        newnewPosition += newPosition.z * pivitPoint.transform.forward;

        //transform.position = newnewPosition ;
        targetPosition = newnewPosition;

        // get the current andgle betwn this and its target.
    //    print( "Angle (rad): " + Mathf.Atan2( pivitPoint.position.z - transform.position.z, pivitPoint.position.y - transform.position.y ) );// * Mathf.Rad2Deg );

    }

}
