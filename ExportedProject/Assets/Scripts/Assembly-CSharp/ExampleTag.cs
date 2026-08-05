using HeavyDutyInspector;
using UnityEngine;

public class ExampleTag : MonoBehaviour
{
	[Comment("Use Unity's tag popup box to select a tag and store it in a string. No more typo errors or having to remember tag names.", CommentType.Info, 0)]
	[Tag]
	public string tagToFind;
}
