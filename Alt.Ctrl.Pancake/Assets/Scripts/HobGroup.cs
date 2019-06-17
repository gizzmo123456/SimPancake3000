using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct to group all of the required hob objects (ie. hob knob, hob flame, frying pan ect...) together.
/// </summary>
/// 
[System.Serializable]
public struct HobGroup
{

	public GameObject fryingPan;
	public GameObject hobKnob;
	public GameObject hobFire;
	
	public void SetHobGroupId(int groupId)
	{

		List<BasePanGroup> panGroup = new List<BasePanGroup>();

		// Get all components that derive from BasePanGroup and set the Hob group id
		panGroup.AddRange( fryingPan.GetComponents<BasePanGroup>() );
		panGroup.AddRange( hobKnob.GetComponents<BasePanGroup>()   );
		panGroup.AddRange( hobFire.GetComponents<BasePanGroup>()   );

		for(int i = 0; i < panGroup.Count; i++ )
			panGroup[i].HobGroupId = groupId;

	}

}
