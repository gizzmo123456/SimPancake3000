using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class PancakePhysicsBall : MonoBehaviour
{
    public enum BallType { left, center, right}
    [SerializeField]
    private bool enablePhysics = false;
    private Rigidbody rigid;

    private Dictionary< BallType, PancakePhysicsBall> balls;
    private Dictionary< BallType, float> ballDistance;

    private bool isCenter = false;
    [SerializeField]
    private float strechyness = 0.1f;

    private Vector3 avgPosition;
    private Vector3 positionOffset; // offset from maintain position
    private float maintainDistance; // amount of distance to maintain from the the target Position
    private float maxY;
    [SerializeField]
    private float velMult = 10;

    Vector3 externalForce = Vector3.zero;

    // Debug things
    public PancakePhysicsBall l;
    public PancakePhysicsBall r;
    public PancakePhysicsBall c;

    public bool debug = false;

    private void Awake()
    {

    }

    // init should be called once all 3 balls have been set.
    // so we can work out all the required data.
    public void Init()
    {
        /*Vector3*/ avgPosition = GetAvgPosition();
        maintainDistance = Vector3.Distance(transform.localPosition, avgPosition);

        maxY = Mathf.Abs( ( avgPosition.x + avgPosition.z ) / 8 );
        positionOffset = transform.localPosition - avgPosition;

    }

    // Start is called before the first frame update
    void Start()
    {

        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = enablePhysics;

    }

    float lastYVel = 0;

    // Update is called once per frame
    void FixedUpdate()
    {

        if( !enablePhysics || isCenter )
            return;
        

        Vector3 velocity = rigid.velocity;
        if ( debug )
            print( "-----# "+velocity );
        Vector3 targetPosition = GetAvgPosition();
        Vector3 posDif = targetPosition - transform.localPosition;

        // Needed ??
        float currentDist = (Vector3.Distance( transform.localPosition, targetPosition ));
        float distDiff = currentDist - maintainDistance; // from maintain distance 

        ///////// Y
        
        // find how much Y distance we are from 0
        float yDistDiff = Mathf.Abs( transform.localPosition.y - targetPosition.y );
        float yDiffPercent = /*1f -*/ ( yDistDiff / maxY );

        float yVel = -yDiffPercent * Physics.gravity.y * Time.deltaTime;
        yVel = transform.localPosition.y >= targetPosition.y ? -yVel : yVel;

        if ( debug )
            print( "%: " + yDiffPercent + " # yVel: " + yVel +" # distDif: "+distDiff + " # yDistDif: "+yDistDiff);

        yVel = velocity.y - yVel;
        if ( debug )
            print( " # yVel (after): " + yVel + " # External: "+externalForce );

        velocity.y -= ( yVel + -externalForce.y);

        /////////// OK no for the x & z Bit :|
        
        // could do both x & z together here :)
        Vector3 max_pos = targetPosition + positionOffset;
        Vector3 target_pos = Vector3.LerpUnclamped( max_pos, targetPosition, ( yDiffPercent ) );
        Vector3 target_percent = Vector3.zero;

        if( ( max_pos.x - target_pos.x ) != 0)
            target_percent.x = ( target_pos.x - transform.localPosition.x ) / ( 1.0f + Mathf.Abs(max_pos.x - target_pos.x)); //??

        if ( ( max_pos.z - target_pos.z ) != 0 )
            target_percent.z = ( target_pos.z - transform.localPosition.z ) / ( 1.0f + Mathf.Abs( max_pos.z - target_pos.z ) ); //??

        if ( debug )
            print( "X %: "+target_percent + " ## "+( target_pos.x - transform.localPosition.x )+" / "+(1+Mathf.Abs( max_pos.x - target_pos.x )) +" ## Target Pos: "+targetPosition +" # X: "+target_pos.x);

        velocity.x += target_percent.x * Time.deltaTime;
        velocity.z += target_percent.z * Time.deltaTime;

//        velocity.y += ( -Physics.gravity.y * Time.deltaTime ) + yVel;  //Anti gravity :)
//        yVel = velocity.y - yVel;
//        velocity.y += yVel;

/*  // GOOD kinda.
        velocity.x = -( yDistDiff / Mathf.Abs( maxY ) ) * positionOffset.x;
        velocity.z = -( yDistDiff / Mathf.Abs( maxY ) ) * positionOffset.z;
*/
        /*
        // do i really need this if statment?
        if ( transform.localPosition.y <= targetPosition.y )
            velocity.y += /*-velocity.y + *//*( -Physics.gravity.y * Time.deltaTime );
        //    velocity.y += ( velMult * distDiff * -Physics.gravity.y)  * Time.deltaTime;  // super streachy
            else
                velocity.y += /*-velocity.y + *//*( -Physics.gravity.y * Time.deltaTime );
        //        velocity.y += ( ( velMult * distDiff ) * Physics.gravity.y ) * Time.deltaTime; // super streachy
        */


        rigid.velocity = velocity;
        lastYVel = rigid.velocity.y;
        if ( debug )
            print( rigid.velocity );

        UpdateExternalForce();

    }

    public void SetBall( BallType type, PancakePhysicsBall physicsBall)
    {
        if ( balls == null )
        {
            balls = new Dictionary<BallType, PancakePhysicsBall>();
            ballDistance = new Dictionary<BallType, float>();
        }

        if ( balls.ContainsKey( type ) )
        {
            balls[ type ] = physicsBall;
            ballDistance[ type ] = Vector3.Distance( transform.position, physicsBall.transform.position );
            print( "Cont" + type );

        }
        else
        {
            balls.Add( type, physicsBall );
            ballDistance.Add( type, Vector3.Distance( transform.position, physicsBall.transform.position ) );
            print( "Add" + type );
        }

        if ( type == BallType.left )
            l = physicsBall;
        else if ( type == BallType.right )
            r = physicsBall;
        else
            c = physicsBall;
    }

    private Vector3 GetAvgPosition()
    {
        if ( isCenter )
            return Vector3.zero;
        else
            return ( balls[ BallType.left ].transform.localPosition + balls[ BallType.right ].transform.localPosition ) / 2f;
    }

    public void SetIsCenter(bool center)
    {
        isCenter = center;
    }

    public void AddForce(float x, float y, float z)
    {
        // is < 0 a good idea on y??
        AddForce( new Vector3( x, y, z ) );
    }

    public void AddForce(Vector3 force)
    {
        // is < 0 a good idea on y??
        externalForce += force;
        print( "FORCE ADDED: " + force );
    }

    private void UpdateExternalForce()  //Y ONLY atm.
    {
        if ( externalForce.y > 0 )
            externalForce.y += Physics.gravity.y * Time.deltaTime;
        else
            externalForce.y = 0;
    }
}
