using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class ExampleReorderableListComponentSelection : MonoBehaviour
{
	[Comment("When applied to a list of objects extending the Component class, reorderable list will display these references with the ComponentSelection drawer.", CommentType.Info, 0)]
	[ReorderableList(true)]
	public List<FakeState> states;
}
