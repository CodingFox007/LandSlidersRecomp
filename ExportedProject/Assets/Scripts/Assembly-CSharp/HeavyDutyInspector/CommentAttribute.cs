using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class CommentAttribute : PropertyAttribute
	{
		public string comment { get; private set; }

		public CommentAttribute(string comment, CommentType messageType = CommentType.None, int order = 0)
		{
			this.comment = comment;
			base.order = order;
		}
	}
}
