using HeavyDutyInspector;
using UnityEngine;

public class ExampleChangeCheckCallback : NamedMonoBehaviour
{
	[Comment("Use the ChangeCheckCallback attribute to get notified when a variable changes. Try it and change the Target variable.", CommentType.Info, 0)]
	[HideVariable]
	public bool comment;

	[ChangeCheckCallback("UpdateName")]
	public GameObject target;

	private void UpdateName()
	{
		scriptName = "Waypoint (" + target.ToString() + ")";
	}
}
