using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGroupManager : MonoBehaviour
{

	[SerializeField] private HobGroup[] hobGroups;
	
	private void Awake()
	{
		for ( int i = 0; i < hobGroups.Length; i++ )
			hobGroups[ i ].SetHobGroupId( i );
	}

	public HobGroup GetHobGroup(int hobId)
	{

		return hobGroups[ hobId ];
	}

}
