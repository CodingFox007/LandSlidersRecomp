using HeavyDutyInspector;
using UnityEngine;

public class ExampleHideConditional : MonoBehaviour
{
	public enum TARGET_TYPE
	{
		Self = 0,
		Position = 1,
		Object = 2
	}

	[Comment("Use the HiddenConditional attribute to hide some of your variables until a given boolean or enumeration condition is achieved.", CommentType.Info, 0)]
	public bool hasTarget;

	[HideConditional("hasTarget", true)]
	public TARGET_TYPE targetType;

	[HideConditional("targetType", new int[] { 1 })]
	public Vector3 positionTarget;

	[HideConditional("targetType", new int[] { 2 })]
	public GameObject objectTarget;
}
