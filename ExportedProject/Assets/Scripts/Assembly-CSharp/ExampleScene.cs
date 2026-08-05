using HeavyDutyInspector;
using UnityEngine;

public class ExampleScene : MonoBehaviour
{
	[Comment("Chose a scene from all the scenes in your project, sorted by folder.", CommentType.Info, 0)]
	public Scene myScene;

	[Comment("Or specify a folder where you want to start searching for scenes.", CommentType.Info, 0)]
	[Scene("Illogika/HeavyDutyInspector/Examples/Scenes")]
	public Scene sortedScenes;
}
