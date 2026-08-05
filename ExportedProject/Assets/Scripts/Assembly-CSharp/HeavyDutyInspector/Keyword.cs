using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class Keyword
	{
		[SerializeField]
		[HideInInspector]
		protected string _key;

		public string category
		{
			get
			{
				if (_key.LastIndexOf('/') < 0)
				{
					return string.Empty;
				}
				return _key.Substring(0, _key.LastIndexOf('/'));
			}
		}

		public string fullPath
		{
			get
			{
				return _key;
			}
		}

		public Keyword()
		{
			_key = string.Empty;
		}

		protected Keyword(string key)
		{
			_key = key ?? string.Empty;
		}

		public static implicit operator string(Keyword word)
		{
			return (word != null) ? word._key.Split('/').LastOrDefault() : string.Empty;
		}

		public static implicit operator Keyword(string key)
		{
			return new Keyword(key);
		}

		public override string ToString()
		{
			return this;
		}
	}
}
