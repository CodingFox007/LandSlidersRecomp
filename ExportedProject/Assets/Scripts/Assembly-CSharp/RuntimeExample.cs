using UnityEngine;

public class RuntimeExample : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 150f, 40f), "Combine All"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().CombineAllMeshRuntime();
		}
		if (GUI.Button(new Rect(170f, 10f, 150f, 40f), "Un Combine All"))
		{
			GameObject.Find("proMeshCombineUtility").GetComponent<proMeshCombineUtility>().UnCombineAllMeshRuntime();
		}
	}
}
