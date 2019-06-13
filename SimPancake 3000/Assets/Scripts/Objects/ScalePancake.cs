using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePancake : MonoBehaviour
{
	[SerializeField] private Transform rootNode;
	[SerializeField] private Vector3 scale = Vector3.one;

	private List<ScaleNode> nodes;

    // Start is called before the first frame update
    void Start()
    {
		nodes = new List<ScaleNode>();
		// recursivly get all the local positions for each nodes
		GetNodes( rootNode );
    }

    // Update is called once per frame
    void Update()
    {
     
		for(int i = 0; i < nodes.Count; i++ )
		{
			Vector3 newPosition = Vector3.zero;


			newPosition.x = nodes[ i ].position.x * scale.x;
			newPosition.y = nodes[ i ].node.localPosition.y;
			newPosition.z = nodes[ i ].position.z * scale.z;

			nodes[ i ].node.localPosition = newPosition;
		}

    }

	void GetNodes(Transform trans)
	{

		// add itself
		AddNode(trans, trans.localPosition);

		for( int i = 0; i < trans.childCount; i++ )
			GetNodes( trans.GetChild( i ) );
		

	}

	private void AddNode(Transform trans, Vector3 localPosition)
	{

		nodes.Add(new ScaleNode(trans, localPosition));

	//	nodes[ nodes.Count - 1 ].SetNode( trans, localPosition );

	}

	[System.Serializable]
	public struct ScaleNode
	{

		public Transform node;
		public Vector3 position;

		public ScaleNode( Transform trans, Vector3 pos)
		{
			print( trans.name + " ## " + pos );
			node = trans;
			position = pos;

		}

	}

}
