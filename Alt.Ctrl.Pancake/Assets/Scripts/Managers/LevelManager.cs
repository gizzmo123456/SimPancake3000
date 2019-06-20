using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public HobGroupManager hobGroupManager;


	private void Awake()
	{
		GameGlobals.levelManager = this;
	}
}
