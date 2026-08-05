using HeavyDutyInspector;
using UnityEngine;

public class ExampleAssetPath : MonoBehaviour
{
	[Comment("Don't let typing errors slow you down. Slide an asset in what looks like a reference and store its path as a string instead. The path is also convieniently displayed under the reference in a selectable Label to allow easy Copy/Pasting.", CommentType.Info, 0)]
	[AssetPath(typeof(Texture2D), PathOptions.RelativeToAssets)]
	public string illogikaLogoPath;

	[AssetPath(typeof(Texture2D), PathOptions.RelativeToResources)]
	public string heavyDutyInspectorLogoPath;

	[AssetPath(typeof(TextAsset), PathOptions.FilenameOnly)]
	public string scriptTemplatePath;
}
