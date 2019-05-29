using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Builds the shape of the pancake and update the shap of the pancake.
 *
 * 
 */
public class PancakeMeshCtrl : MonoBehaviour
{

    private Mesh mesh;
    [SerializeField]
    private Vector3[] vertices;

    public List<VerticeGroup> verticeGroups;

    [SerializeField]
    private PancakePhysicsBall physicsBall;
    int centerId = -1;  //so we can set it on physics ball later

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        verticeGroups = new List<VerticeGroup>();


        bool found = false;
       
        /*
         * group all vert's, so anythink that has the same x, z value can be moved together.
         * we are also going to put a point (or sphere) on each group which will be used for are physics
         * we want the two faces of the cylinder (or pancake) to move together.
         */

        // loop all vert's
        for ( int i = 0; i < vertices.Length; i++ )
        {
            found = false;

            // find if there is an existing group
            for( int j = 0; j < verticeGroups.Count; j++ )
            {
                if (verticeGroups[j].Compare(vertices[i].x, vertices[i].z))
                {
                    verticeGroups[ j ].AddVertID( i );
                    found = true;

                    break;
                }
            }

            
            if( !found ) // add spwan physics ball and add new group.
            {
                PancakePhysicsBall pb = Instantiate( physicsBall, transform );
                verticeGroups.Add( new VerticeGroup( i, vertices[ i ].x, vertices[ i ].z, pb) );

                pb.transform.localPosition = verticeGroups[ verticeGroups.Count - 1 ].GetAsVector3();

                // update center id once it has been found (it should always be 0,0)
                if ( centerId == -1 && vertices[ i ].x == 0 && vertices[ i ].z == 0 )
                {
                    centerId = verticeGroups.Count - 1;
                }
            }

        }


        verticeGroups[ centerId ].physicsBall.SetIsCenter( true );
        // now we have all our groups we can, set the center and two cloest physics balls to each physics ball
        for ( int i = 0; i < verticeGroups.Count; i++ )
        {
            verticeGroups[ i ].physicsBall.SetBall( PancakePhysicsBall.BallType.center, verticeGroups[ centerId ].physicsBall );
            // find the two closes balls that are not the center.
            List<int> cloestId = new List<int>( 2 );
            cloestId.AddRange( new int[ 2 ] { 0, 0 } );

            List<float> dist = new List<float>( 2 );
            dist.AddRange( new float[ 2 ] { 100f, 100f } );

            verticeGroups[ i ].physicsBall.name = i + "";
            print( i + " center dist: " + Vector3.Distance( verticeGroups[ i ].GetAsVector3(), verticeGroups[ centerId ].GetAsVector3() ) );


            for ( int j = 0; j < verticeGroups.Count; j++ )
            {

                if ( verticeGroups[ j ].GetAsVector3() == Vector3.zero || i == j ) // skip center and self
                    continue;

                float tempDist = Vector3.Distance( verticeGroups[ i ].GetAsVector3(), verticeGroups[ j ].GetAsVector3() );
                if ( j == 0 || tempDist < dist[ 0 ] )
                {
                    cloestId.Insert( 0, j );
                    dist.Insert( 0, tempDist );
                }
                else if( j == 1 || tempDist < dist[1] )
                {
                    cloestId.Insert( 1, j );
                    dist.Insert( 1, tempDist );
                }

            }

            // set the two balls now we have found the cloest two
            verticeGroups[ i ].physicsBall.SetBall( PancakePhysicsBall.BallType.left, verticeGroups[ cloestId[0] ].physicsBall );
            verticeGroups[ i ].physicsBall.SetBall( PancakePhysicsBall.BallType.right, verticeGroups[ cloestId[1] ].physicsBall );
            verticeGroups[ i ].physicsBall.Init();

        }
        

    }

    void FixedUpdate()
    {
        //return;
        for ( int i = 0; i < verticeGroups.Count; i++ )
        {
            Vector3 pbPosition = verticeGroups[ i ].physicsBall.transform.localPosition;

            verticeGroups[ i ].Update( ref vertices, pbPosition.x, pbPosition.y, pbPosition.z );
        }
    }

    private void Update()
    {
        mesh.vertices = vertices;   // << am i fucking crazy
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
    }

}

[System.Serializable]
public struct VerticeGroup
{
	private float x, z;   // we dont need the Y asix since we only need to change the shape of its face and the top will have a matching bottom.
	private List<int> verticesIDs;
	public PancakePhysicsBall physicsBall;

	public VerticeGroup( int vertID, float xAxis, float zAxis, PancakePhysicsBall pb )
	{
		x = xAxis;
		z = zAxis;
		physicsBall = pb;
		verticesIDs = new List<int>();
		verticesIDs.Add( vertID );
	}

	public void Update( ref Vector3[] verts, float xAxis, float yAxis, float zAxis )
	{

		x = xAxis;
		z = zAxis;

		foreach ( int vid in verticesIDs )
		{
			verts[ vid ].x = x;
			verts[ vid ].z = z;
			verts[ vid ].y = yAxis;
		}

	}

	public void AddVertID( int id )
	{
		verticesIDs.Add( id );
	}

	public Vector3 GetAsVector3()
	{
		return new Vector3( x, 0, z );
	}

	public bool Compare( float xAxis, float zAxis )
	{
		return x == xAxis && z == zAxis;
	}

}