using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePanGroup : MonoBehaviour
{
	
	public virtual int PanGroupId { get; set; }
	protected InputHandler Inputs { get { return GameGlobals.inputs; } }
	//protected ... HobGroupManager { get {GameGlobals.hobManager;} }	//TODO: 


}
