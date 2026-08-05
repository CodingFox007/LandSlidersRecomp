using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	public class NamedMonoBehaviourAttribute : PropertyAttribute
	{
		[Obsolete("Use ComponentSelection instead.")]
		public NamedMonoBehaviourAttribute()
		{
		}
	}
}
