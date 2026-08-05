using HeavyDutyInspector;
using UnityEngine;

public class ExampleButton : MonoBehaviour
{
	[Comment("Add buttons and call functions by name using reflection", CommentType.Info, 0)]
	[Button("Reset Position", "ResetPosition", true)]
	public bool hidden;

	[Comment("Or create buttons with an image by specifying its path.", CommentType.Info, 0)]
	[ImageButton("Illogika/HeavyDutyInspector/Examples/Textures/illogika-logo.png", "ResetPosition", true)]
	public bool hidden2;

	private void ResetPosition()
	{
		Debug.Log("ResetPosition");
		base.transform.position = Vector3.zero;
	}
}
