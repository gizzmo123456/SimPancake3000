using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePanGroup : MonoBehaviour
{
	
	public virtual int HobGroupId { get; set; }
	protected InputHandler Inputs { get { return GameGlobals.inputs; } }
	[SerializeField] protected float inputValueOffset = 0;
	//protected ... HobGroupManager { get {GameGlobals.hobManager;} }	//TODO: 

	protected abstract void UpdateInputValues();


}
