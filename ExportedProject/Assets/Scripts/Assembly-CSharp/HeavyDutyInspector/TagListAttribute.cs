using UnityEngine;

namespace HeavyDutyInspector
{
	public class TagListAttribute : PropertyAttribute
	{
		public bool canDeleteFirstElement { get; private set; }

		public TagListAttribute(bool canDeleteFirstElement = true)
		{
			this.canDeleteFirstElement = canDeleteFirstElement;
		}
	}
}
