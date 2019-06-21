using UnityEngine;


public interface IPanChanged
{

	void OnPanChanged( int panId );

}

public interface IPanCollider
{
	void SetPanCollider(Transform gameObj);
}