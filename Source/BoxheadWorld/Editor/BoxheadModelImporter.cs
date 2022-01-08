using UnityEditor;

//Tutorial:https://blog.csdn.net/lsjsoft/article/details/106367930
//Doc:https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html

public class BoxheadModelImporter : AssetPostprocessor
{
    //Doc:https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessModel.html
    private void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        if (importer == null)
        {
            return;
        }

        string name = importer.assetPath.ToLower();
        string extension = name.Substring(name.LastIndexOf(".")).ToLower();

        switch (extension)
        {
            case ".fbx":
                importer.materialLocation = ModelImporterMaterialLocation.External;
                importer.materialSearch = ModelImporterMaterialSearch.Local;
                break;
            default:
                break;
        }
    }
}