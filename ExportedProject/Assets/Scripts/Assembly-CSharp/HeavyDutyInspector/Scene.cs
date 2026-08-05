using System;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	[Serializable]
	public class Scene
	{
		[SerializeField]
		[HideInInspector]
		private string _name;

		public string fullPath
		{
			get
			{
				return _name;
			}
		}

		public Scene()
		{
			_name = string.Empty;
		}

		private Scene(string name)
		{
			_name = name;
		}

		public static implicit operator string(Scene scene)
		{
			return scene._name.Split('/').Last().Replace(".unity", string.Empty);
		}

		public static implicit operator Scene(string path)
		{
			return new Scene(path);
		}
	}
}
