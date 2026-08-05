using UnityEngine;

public class RuntimeExample2 : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 150f, 40f), "Clean Combine ALL"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineAllMeshRuntime();
		}
		if (GUI.Button(new Rect(10f, 70f, 150f, 40f), "Combine level 1"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineCustomRuntime(GameObject.Find("level1"), false, true);
		}
		if (GUI.Button(new Rect(10f, 130f, 150f, 40f), "Combine level 2"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineCustomRuntime(GameObject.Find("level2"), false, true);
		}
		if (GUI.Button(new Rect(10f, 200f, 150f, 40f), "Combine level 3"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineCustomRuntime(GameObject.Find("level3"), false, true);
		}
		if (GUI.Button(new Rect(10f, 260f, 150f, 40f), "Combine level 4"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineCustomRuntime(GameObject.Find("level4"), false, true);
		}
	}
}
