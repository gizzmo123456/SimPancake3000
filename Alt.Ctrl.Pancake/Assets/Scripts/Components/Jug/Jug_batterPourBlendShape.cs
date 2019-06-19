using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class Jug_batterPourBlendShape : BasePanGroup_singleInput
{
	private SkinnedMeshRenderer skinnedMeshRenderer;

	[Header( "Pour BlendShape" )]
	[SerializeField] private int pour_blendShapeId = 0;
	private bool active = false;

	private void Awake()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		//GetComponent<Jug_panSellect>().OnPanChanged += OnPanChanged;
	}

	protected override void Update()
    {

		base.Update();

		float blendAmount = (1f - inputValue.ClampedPrecent) * 100f;

		skinnedMeshRenderer.SetBlendShapeWeight( pour_blendShapeId, blendAmount );

    }

	public void OnPanChanged( int id )
	{
		active = id > -1;
	}
}
