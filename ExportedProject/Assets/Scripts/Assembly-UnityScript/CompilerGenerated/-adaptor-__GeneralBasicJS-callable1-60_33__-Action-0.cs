using System;
using Boo.Lang.Runtime;
using UnityEngine;

namespace CompilerGenerated
{
	[Serializable]
	internal sealed class _0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240
	{
		protected __GeneralBasicJS_0024callable1_002460_33__ _0024from;

		public _0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240(__GeneralBasicJS_0024callable1_002460_33__ from)
		{
			_0024from = from;
		}

		public void Invoke(object obj)
		{
			__GeneralBasicJS_0024callable1_002460_33__ _GeneralBasicJS_0024callable1_002460_33__ = _0024from;
			object obj2 = obj;
			if (!(obj2 is GameObject))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(GameObject));
			}
			_GeneralBasicJS_0024callable1_002460_33__((GameObject)obj2);
		}

		public static Action<object> Adapt(__GeneralBasicJS_0024callable1_002460_33__ from)
		{
			return new _0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240(from).Invoke;
		}
	}
}
