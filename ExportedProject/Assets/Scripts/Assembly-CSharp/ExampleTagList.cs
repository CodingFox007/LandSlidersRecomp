using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class ExampleTagList : MonoBehaviour
{
	[Comment("Use Unity's tag popup box to select tags in a list of strings. Allows you to delete a tag from anywhere in the list.", CommentType.Info, 0)]
	[TagList(true)]
	public List<string> tagsToFind;
}
