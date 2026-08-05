using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class UInt32S
	{
		[SerializeField]
		private byte[] _bytes;

		[SerializeField]
		private bool _endianness;

		public UInt32S()
		{
			_bytes = BitConverter.GetBytes(0u);
			_endianness = BitConverter.IsLittleEndian;
		}

		public UInt32S(uint value)
		{
			_bytes = BitConverter.GetBytes(value);
			_endianness = BitConverter.IsLittleEndian;
		}

		public static implicit operator uint(UInt32S value)
		{
			try
			{
				return BitConverter.ToUInt32((BitConverter.IsLittleEndian != value._endianness) ? value._bytes.Reverse().ToArray() : value._bytes, 0);
			}
			catch
			{
				return 0u;
			}
		}

		public static implicit operator UInt32S(uint value)
		{
			return new UInt32S(value);
		}
	}
}
