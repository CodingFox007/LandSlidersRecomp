using HeavyDutyInspector;
using UnityEngine;

public class ExampleImage : MonoBehaviour
{
	[Comment("Add images or logos in the Inspector", CommentType.Info, 0)]
	[Image("Illogika/HeavyDutyInspector/Examples/Textures/illogika-logo.png", Alignment.Center, 1)]
	[Image("Assets/Illogika/HeavyDutyInspector/Examples/Textures/Resources/Textures/heavy_duty_logo.png", Alignment.Center, 2)]
	[HideVariable]
	public bool hidden;
}
