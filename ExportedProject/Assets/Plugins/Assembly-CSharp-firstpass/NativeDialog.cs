using UnityEngine;

public class NativeDialog
{
	private static string GetGameObjectPath(GameObject gameObject)
	{
		if (gameObject != null)
		{
			Transform transform = gameObject.transform;
			string text = transform.name;
			while (transform.parent != null)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return text;
		}
		return string.Empty;
	}

	public static void ShowDialog(string title, string msg, string ok)
	{
	}

	public static void ShowRateItNowDialogWithResponder(string title, string msg, string doItText, string remindMeLaterText, string neverText, GameObject responder)
	{
	}
}
