using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class UInt16S
	{
		[SerializeField]
		private byte[] _bytes;

		[SerializeField]
		private bool _endianness;

		public UInt16S()
		{
			_bytes = BitConverter.GetBytes((ushort)0);
			_endianness = BitConverter.IsLittleEndian;
		}

		public UInt16S(ushort value)
		{
			_bytes = BitConverter.GetBytes(value);
			_endianness = BitConverter.IsLittleEndian;
		}

		public static implicit operator ushort(UInt16S value)
		{
			try
			{
				return BitConverter.ToUInt16((BitConverter.IsLittleEndian != value._endianness) ? value._bytes.Reverse().ToArray() : value._bytes, 0);
			}
			catch
			{
				return 0;
			}
		}

		public static implicit operator UInt16S(ushort value)
		{
			return new UInt16S(value);
		}
	}
}
