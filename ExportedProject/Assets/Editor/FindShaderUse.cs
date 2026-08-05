using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FindShaderUse : EditorWindow
{
    private Shader targetShader;
    private List<string> usedMaterials = new List<string>();
    private Vector2 scrollPos;

    [MenuItem("Window/Find Shader Use")]
    public static void ShowWindow()
    {
        GetWindow<FindShaderUse>("Find Shader Use");
    }

    private void OnGUI()
    {
        targetShader = (Shader)EditorGUILayout.ObjectField("Target Shader", targetShader, typeof(Shader), false);

        if (GUILayout.Button("Find Materials Using Shader") && targetShader != null)
        {
            usedMaterials.Clear();
            string shaderPath = AssetDatabase.GetAssetPath(targetShader);
            string[] allMaterialGuids = AssetDatabase.FindAssets("t:Material");

            foreach (string guid in allMaterialGuids)
            {
                string matPath = AssetDatabase.GUIDToAssetPath(guid);
                string[] dependencies = AssetDatabase.GetDependencies(matPath);

                if (ArrayUtility.Contains(dependencies, shaderPath))
                {
                    usedMaterials.Add(matPath);
                }
            }
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (string matPath in usedMaterials)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(matPath));
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                Selection.activeObject = mat;
                EditorGUIUtility.PingObject(mat);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
