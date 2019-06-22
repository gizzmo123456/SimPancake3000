using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceivePancake
{

	void AddPancake(Pancake_state pancake);
	void RemovePancake(Pancake_state pancake);

}