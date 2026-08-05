using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	public class AssetPathAttribute : PropertyAttribute
	{
		public UnityEngine.Object obj { get; set; }

		public Type type { get; private set; }

		public PathOptions pathOptions { get; private set; }

		public int folderDepth { get; private set; }

		public string actualAssetVariable { get; private set; }

		public AssetPathAttribute(Type type, PathOptions pathOptions)
		{
			folderDepth = -1;
			this.pathOptions = pathOptions;
			this.type = type;
		}

		public AssetPathAttribute(PathOptions pathOptions)
		{
			folderDepth = -1;
			this.pathOptions = pathOptions;
			type = typeof(UnityEngine.Object);
		}

		public AssetPathAttribute(Type type, int folderDepth)
		{
			this.folderDepth = folderDepth;
			if (folderDepth == 0)
			{
				pathOptions = PathOptions.FilenameOnly;
			}
			else
			{
				pathOptions = PathOptions.RelativeToAssets;
			}
			this.type = type;
		}

		public AssetPathAttribute(int folderDepth)
		{
			this.folderDepth = folderDepth;
			if (folderDepth == 0)
			{
				pathOptions = PathOptions.FilenameOnly;
			}
			else
			{
				pathOptions = PathOptions.RelativeToAssets;
			}
			type = typeof(UnityEngine.Object);
		}
	}
}
