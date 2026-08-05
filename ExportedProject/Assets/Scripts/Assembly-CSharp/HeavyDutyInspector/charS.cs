using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class charS
	{
		[SerializeField]
		private byte[] _bytes;

		public charS()
		{
			_bytes = BitConverter.GetBytes('\0');
		}

		public charS(char value)
		{
			_bytes = BitConverter.GetBytes(value);
		}

		public static implicit operator char(charS value)
		{
			try
			{
				return BitConverter.ToChar(value._bytes, 0);
			}
			catch
			{
				return '\0';
			}
		}

		public static implicit operator charS(char value)
		{
			return new charS(value);
		}

		public static implicit operator string(charS value)
		{
			return new string(new char[1] { value });
		}
	}
}
