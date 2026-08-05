using HeavyDutyInspector;
using UnityEngine;

public class ExampleLayer : MonoBehaviour
{
	[Comment("Easily select a layer from Unity's layer popup and store it as an integer", CommentType.Info, 0)]
	[Layer]
	public int affectedLayer;
}
