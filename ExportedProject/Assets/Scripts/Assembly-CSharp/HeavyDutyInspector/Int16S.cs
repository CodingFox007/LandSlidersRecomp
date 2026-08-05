using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class Int16S
	{
		[SerializeField]
		private byte[] _bytes;

		[SerializeField]
		private bool _endianness;

		public Int16S()
		{
			_bytes = BitConverter.GetBytes((short)0);
			_endianness = BitConverter.IsLittleEndian;
		}

		public Int16S(short value)
		{
			_bytes = BitConverter.GetBytes(value);
			_endianness = BitConverter.IsLittleEndian;
		}

		public static implicit operator short(Int16S value)
		{
			try
			{
				return BitConverter.ToInt16((BitConverter.IsLittleEndian != value._endianness) ? value._bytes.Reverse().ToArray() : value._bytes, 0);
			}
			catch
			{
				return 0;
			}
		}

		public static implicit operator Int16S(short value)
		{
			return new Int16S(value);
		}
	}
}
