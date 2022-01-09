using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BoxheadEditor : EditorWindow
{
    //Doc:https://docs.unity3d.com/ScriptReference/AssetDatabase.IsValidFolder.html
    [MenuItem(nameof(BoxheadEditor) + "/" + nameof(CreateFolderStructure))]
    private static void CreateFolderStructure()
    {
        //create Resources folder:
        bool isResourcesValid = AssetDatabase.IsValidFolder("Assets/Resources");
        if (!isResourcesValid)
        {
            if (AssetDatabase.CreateFolder("Assets", "Resources") != string.Empty)
            {
                Debug.Log($"{nameof(BoxheadEditor)}: Create " + "Assets/Resources" + " success!");
            }
        }

        //create folder structures:
        List<string> subFolders = new List<string>() { "Prefabs", "Models" };
        foreach (string item in subFolders)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/" + item))
            {
                if (AssetDatabase.CreateFolder("Assets/Resources", item) != string.Empty)
                {
                    Debug.Log($"{nameof(BoxheadEditor)}: Create " + "Assets/Resources/" + item + " success!");
                }
            }
        }
    }

    [MenuItem(nameof(BoxheadEditor) + "/" + nameof(Window))]
    private static void Window()
    {
        BoxheadEditor window = GetWindow<BoxheadEditor>();
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label($"Welcome to {nameof(BoxheadEditor)}!");
        bool click = GUILayout.Button("CLICK");
        if (click)
        {
            Debug.Log("CLICKED");
        }

        EditorGUILayout.EndHorizontal();
    }
}