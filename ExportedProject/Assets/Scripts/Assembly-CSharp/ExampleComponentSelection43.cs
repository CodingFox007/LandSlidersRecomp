using HeavyDutyInspector;
using UnityEngine;

public class ExampleComponentSelection43 : MonoBehaviour
{
	[Comment("Select components through a drop down menu that displays numbered components to easily identify which is which on overcharged GameObjects. Specify the name of a field belonging to this component to have its value displayed after the component's type and numbering, making it even easier to know which component you want to select.", CommentType.Info, 0)]
	[ComponentSelection("clip")]
	public AudioSource footstepsAudioSource;

	[Comment("Use the ComponentSelection Attribute to choose which component you want to select on another GameObject without having to open a second inspector.", CommentType.Info, 0)]
	[ComponentSelection]
	public FakeState idleState;

	[Comment("New in Unity 4.3! Use the ComponentSelection Attribute with arrays or lists.", CommentType.Info, 0)]
	[ComponentSelection]
	public FakeStateAttack[] attackStates;
}
