using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AMS_Helpers;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class Jug_batterPourBlendShape : BasePanGroup_singleInput, IPanChanged
{
	private SkinnedMeshRenderer skinnedMeshRenderer;
	private bool active = false;

	[Header( "Pour BlendShape" )]
	[SerializeField] private int pour_blendShapeId = 0;

	private void Awake()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		GetComponentInParent<Jug_panSellect>().OnPanChanged += OnPanChanged;
	}

	protected override void Update()
    {

		if ( !active ) return;

		base.Update();

		float blendAmount = (1f - inputValue.ClampedPrecent) * 100f;

		skinnedMeshRenderer.SetBlendShapeWeight( pour_blendShapeId, blendAmount );

    }

	public void OnPanChanged( int id )
	{
		active = id > -1;

		// reset blend shape back to start if in default location (-1)
		if ( id < 0 )
			skinnedMeshRenderer.SetBlendShapeWeight( pour_blendShapeId, 0 );
	}
}
