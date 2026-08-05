using HeavyDutyInspector;
using UnityEngine;

public class ExampleComment : MonoBehaviour
{
	[Comment("Quickly add comments to your variables.", CommentType.Info, 0)]
	public bool isCommented = true;
}
