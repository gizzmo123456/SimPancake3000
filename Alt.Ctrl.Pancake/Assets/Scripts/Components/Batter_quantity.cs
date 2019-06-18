using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batter_quantity : BasePanGroup
{

	public delegate void OnBatter( float batterPrecentage );
	public event OnBatter OnBatterChanged;

	[SerializeField] private float maxBatterQuantity = 100;
	[SerializeField] private bool setMaxOnStart = true;

	private float currentBatterQuantity;

	private void Start()
	{
		if(setMaxOnStart)
			currentBatterQuantity = maxBatterQuantity;
	}

	/// <summary>
	/// Add batter to quantiity
	/// </summary>
	/// <param name="amountOfBatterToAdd"></param>
	/// <returns>returns remain batter over max</returns>
	public float AddBatter(float amountOfBatterToAdd)
	{

		float remainingBatter = 0f;
		currentBatterQuantity += amountOfBatterToAdd;

		if ( currentBatterQuantity > maxBatterQuantity )
		{
			remainingBatter = currentBatterQuantity - maxBatterQuantity;
			amountOfBatterToAdd -= remainingBatter;
			currentBatterQuantity = maxBatterQuantity;
		}

		OnBatterChanged?.Invoke( GetBatterPrecent() );

		return remainingBatter;

	}

	/// <summary>
	/// uses batter for it quantity and returns the amount of batter that is available.
	/// </summary>
	/// <param name="amountOfBatterToUse"></param>
	/// <returns>The amount of batter that can be used.</returns>
	public float UseBatter(float amountOfBatterToUse)
	{

		if ( currentBatterQuantity == 0 )							// No batter remaining :(
		{
			amountOfBatterToUse = 0;
		}
		else if ( currentBatterQuantity > amountOfBatterToUse )		// theres enought batter to fulfil the request :)
		{ 
			currentBatterQuantity -= amountOfBatterToUse;
		}
		else														// theres not enought batter for that, heres what remains :|
		{
			amountOfBatterToUse = currentBatterQuantity;
			currentBatterQuantity = 0;
		}

		OnBatterChanged?.Invoke( GetBatterPrecent() );

		return amountOfBatterToUse;

	}

	public bool HasBatter()
	{
		return currentBatterQuantity > 0f;
	}

	public float GetBatterQuantity()
	{
		return currentBatterQuantity;
	}

	public float GetBatterPrecent()
	{
		return currentBatterQuantity / maxBatterQuantity;
	}

}
