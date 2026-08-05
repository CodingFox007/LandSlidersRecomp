using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class Int64S
	{
		[SerializeField]
		private byte[] _bytes;

		[SerializeField]
		private bool _endianness;

		public Int64S()
		{
			_bytes = BitConverter.GetBytes(0L);
			_endianness = BitConverter.IsLittleEndian;
		}

		public Int64S(long value)
		{
			_bytes = BitConverter.GetBytes(value);
			_endianness = BitConverter.IsLittleEndian;
		}

		public static implicit operator long(Int64S value)
		{
			try
			{
				return BitConverter.ToInt64((BitConverter.IsLittleEndian != value._endianness) ? value._bytes.Reverse().ToArray() : value._bytes, 0);
			}
			catch
			{
				return 0L;
			}
		}

		public static implicit operator Int64S(long value)
		{
			return new Int64S(value);
		}
	}
}
