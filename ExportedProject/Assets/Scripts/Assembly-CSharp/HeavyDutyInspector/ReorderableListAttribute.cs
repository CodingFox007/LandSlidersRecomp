using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	public class ReorderableListAttribute : PropertyAttribute
	{
		public Texture2D arrowUp { get; private set; }

		public Texture2D arrowDown { get; private set; }

		public Texture2D olPlus { get; private set; }

		public Texture2D olMinus { get; private set; }

		public bool useDefaultComponentDrawer { get; private set; }

		[Obsolete("The parameters doubleComponentRefSizeInChildren and useNamedMonoBehaviourDrawer don't do anything anymore. Use the parameterless constructor instead.")]
		public ReorderableListAttribute(bool doubleComponentRefSizeInChildren, bool useNamedMonoBehaviourDrawer)
		{
		}

		public ReorderableListAttribute(bool useComponentSelectionDrawer = true)
		{
			useDefaultComponentDrawer = !useComponentSelectionDrawer;
		}
	}
}
