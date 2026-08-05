using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(RectTransform))]
public class CurvedText : BaseMeshEffect
{
	public AnimationCurve curveForText = AnimationCurve.Linear(0f, 0f, 1f, 10f);

	public float curveMultiplier = 1f;

	private RectTransform rectTrans;

	protected override void Awake()
	{
		base.Awake();
		rectTrans = GetComponent<RectTransform>();
		OnRectTransformDimensionsChange();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		rectTrans = GetComponent<RectTransform>();
		OnRectTransformDimensionsChange();
	}

	public override void ModifyMesh(VertexHelper mesh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			mesh.GetUIVertexStream(list);
			for (int i = 0; i < list.Count; i++)
			{
				UIVertex value = list[i];
				value.position.y += curveForText.Evaluate(rectTrans.rect.width * rectTrans.pivot.x + value.position.x) * curveMultiplier;
				list[i] = value;
			}
			mesh.Clear();
			mesh.AddUIVertexTriangleStream(list);
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		Keyframe key = curveForText[curveForText.length - 1];
		key.time = rectTrans.rect.width;
		curveForText.MoveKey(curveForText.length - 1, key);
	}
}
