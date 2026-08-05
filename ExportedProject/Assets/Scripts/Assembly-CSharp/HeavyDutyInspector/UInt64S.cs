using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class UInt64S
	{
		[SerializeField]
		private byte[] _bytes;

		[SerializeField]
		private bool _endianness;

		public UInt64S()
		{
			_bytes = BitConverter.GetBytes(0uL);
			_endianness = BitConverter.IsLittleEndian;
		}

		public UInt64S(ulong value)
		{
			_bytes = BitConverter.GetBytes(value);
			_endianness = BitConverter.IsLittleEndian;
		}

		public static implicit operator ulong(UInt64S value)
		{
			try
			{
				return BitConverter.ToUInt64((BitConverter.IsLittleEndian != value._endianness) ? value._bytes.Reverse().ToArray() : value._bytes, 0);
			}
			catch
			{
				return 0uL;
			}
		}

		public static implicit operator UInt64S(ulong value)
		{
			return new UInt64S(value);
		}
	}
}
