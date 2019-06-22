using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVelocity
{
	Vector3 Velocity { get; }

	void AddVelocity( Vector3 vel );
	void SetVelocity( Vector3 vel );
	Vector3 GetVelocity( );

}