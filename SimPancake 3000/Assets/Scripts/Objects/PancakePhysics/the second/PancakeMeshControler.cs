using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Sorts mesh verts into update groups
 * reduces the amount of verts, for less updates
 * adds physics balls to each horizontal vert
 * 
 * Note:
 * we are using the VerticeGroup Struct from the org PancakeMeshContr for this :)
 */

public class PancakeMeshControler : MonoBehaviour
{

	private Mesh mesh;
	private Vector3[] vertices;

	[SerializeField] private List<VerticeGroup> vertGroups; //This does not need to be serialized
	
	private int centerId = -1;

	[SerializeField] private PhysicsBall physicsBall;

	// Some form of physic ball, i still need to work it out :)

	void Awake()
	{

		// get the mesh and all of the verts
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		vertGroups = new List<VerticeGroup>();

		// loop all verts and group them into x, z group.
		for ( int i = 0; i < vertices.Length; i++ )
		{

			int vertGroupId = FindExistingVertGroup( vertices[ i ].x, vertices[ i ].z );

			if ( vertGroupId > -1 ) // add to existing group
			{
				vertGroups[ vertGroupId ].AddVertID( i );
			}
			else // Creat new group
			{

				PhysicsBall tempBall = Instantiate( physicsBall, transform );
				vertGroups.Add( new VerticeGroup( i, vertices[ i ].x, vertices[ i ].z, tempBall ) );    //TODO: change null to physic ball :)
				
				tempBall.transform.localPosition = vertGroups[ vertGroups.Count - 1 ].GetAsVector3();
				tempBall.name = "TB_" + i;

				if( centerId == -1 && vertices[i].x == 0 && vertices[i].z == 0)
				{
					centerId = vertGroups.Count - 1;
					vertGroups[ centerId ].SetCenter(true);
				}

			}
		}

		// so now we have all the verts into groups, we need to find wich groups are to the left and right of the current.

		float distanceFromCenter = 0;

		for ( int i = 0; i < vertGroups.Count; i++ )
		{
			//Skip the center Id, since id does not need a left and right
			if ( i == centerId ) continue;
			// creat two list for the two cloest points that are distance from center.
			List<int> cloestId = new List<int>( 2 );
			List<float> dist = new List<float>( 2 );

			cloestId.AddRange( new int[ 2 ] { 0, 0 } );
			dist.AddRange( new float[ 2 ] { 100f, 100f } );

			distanceFromCenter = Vector3.Distance( Vector3.zero, vertGroups[ i ].GetAsVector3() );

			for( int j = 0; j < vertGroups.Count; j++ )
			{
				// skip cemter and itself
				if ( j == centerId || j == i ) continue;
				
				// we only need the left and right that are on the same ring as the current vert.
				// NOTE TO ME.
				// Keep an eye on this, since it's outside of the current testing scope.
				// reseon being is that "Vector3.Distance( Vector3.zero, vertGroups[ j ].GetAsVector3() ) != distanceFromCenter" was no evelateing correctly
				// some times it would evelate as TRUE when both where the same. Im going to put it down to rounding error :(
				// But still this should be the first point of contact if there are any issues like this in a more rigarus test.
				if ( !Mathf.Approximately( Vector3.Distance( Vector3.zero, vertGroups[ j ].GetAsVector3() ), distanceFromCenter ) ) continue;

				// find if its cloestes
				float tempDistance = Vector3.Distance( vertGroups[ i ].GetAsVector3(), vertGroups[ j ].GetAsVector3() );
	
				for( int cid = 0; cid < cloestId.Count; cid++ )
					if ( j == cid || tempDistance < dist[cid] )
					{
						cloestId.Insert( cid, j );
						dist.Insert( cid, tempDistance );
						break;
					}
			}

			// set it up on a Physic ball
			// TODO: ^^^
			vertGroups[ i ].physicsBall.SetBalls( vertGroups[ cloestId[ 0 ] ].physicsBall, vertGroups[ cloestId[ 1 ] ].physicsBall );

		}

	}

	// returns vert gorup Id, -1 if not found.
	private int FindExistingVertGroup(float x, float z)
	{

		for ( int i = 0; i < vertGroups.Count; i++ )
			if ( vertGroups[ i ].Compare( x, z ) )
				return i;

		return -1;

	}

	public VerticeGroup[] GetVertGroups()
	{
		return vertGroups.ToArray();
	}

	[System.Serializable]
	public struct VerticeGroup
	{
		private float x, z;   // we dont need the Y asix since we only need to change the shape of its face and the top will have a matching bottom.
		private List<int> verticesIDs;
		public PhysicsBall physicsBall;
		public bool isCenter;

		public VerticeGroup( int vertID, float xAxis, float zAxis, PhysicsBall pb )
		{
			x = xAxis;
			z = zAxis;
			physicsBall = pb;
			verticesIDs = new List<int>();
			verticesIDs.Add( vertID );
			isCenter = false;
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

		public void SetCenter( bool b )
		{
			isCenter = b;
		}

		public bool Compare( float xAxis, float zAxis )
		{
			return x == xAxis && z == zAxis;
		}

	}
}
