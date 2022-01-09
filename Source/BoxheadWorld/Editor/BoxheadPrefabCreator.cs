using UnityEngine;
using UnityEditor;

//REF:https://docs.unity3d.com/ScriptReference/PrefabUtility.html

public class BoxheadPrefabCreator
{
    [MenuItem(nameof(BoxheadEditor) + "/" + nameof(CreatePrefab))]
    private static void CreatePrefab()
    {
        // Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        //Check Folder:
        const string defaultPrefabFolder = "Assets/Resources/Prefabs";
        if (!AssetDatabase.IsValidFolder(defaultPrefabFolder))
        {
            Debug.LogWarning($"{defaultPrefabFolder}" + " is not valid! Please create/check it!");
            return;
        }

        // Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            string savePath = $"{defaultPrefabFolder}/{gameObject.name}.prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, savePath, InteractionMode.UserAction, out bool success);
            if (success)
            {
                Debug.Log($"{nameof(BoxheadPrefabCreator)} create new prefab:{savePath}.");
            }
        }
    }

    // Disable the menu item if no selection is in place.
    [MenuItem(nameof(BoxheadEditor) + "/" + nameof(CreatePrefab), true)]
    private static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
}