using System.IO;

using UnityEditor;

using UnityEngine;

//Doc:https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html

public class BoxheadModelImporter : AssetPostprocessor
{
    //Tutorial:https://blog.csdn.net/lsjsoft/article/details/106367930
    //Doc:https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessModel.html
    private void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        FileInfo file = new FileInfo(importer.assetPath);
        switch (file.Extension.ToLower())
        {
            case ".fbx":
                importer.materialLocation = ModelImporterMaterialLocation.External;
                importer.materialSearch = ModelImporterMaterialSearch.Local;
                break;
            default:
                break;
        }
    }

    private void OnPostprocessModel(GameObject gameObject)
    {
        ModelImporter importer = assetImporter as ModelImporter;

        //自动设置所有模型的Transform.Scale，暂时不要使用Scale Factor，会导致播放动画时模型错位:
        //这个值会影响模型的大小, 应该根据具体建模比例设置:
        gameObject.transform.localScale = new Vector3(4, 4, 4);

        //针对特定模型:
        if (gameObject.name == "bambo")
        {
            FileInfo file = new FileInfo(importer.assetPath);
            string filePath = file.ToString();
            string folderPath = filePath.Substring(0, filePath.LastIndexOf('/'));

            //创建动画控制器:https://docs.unity3d.com/ScriptReference/Animations.AnimatorController.html
            string acPath = folderPath + "/" + gameObject.name + ".controller";
            var ac = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(acPath);
            if (ac != null)
            {
                //try to add animation clips:
                string animationObjectPath = folderPath + "/" + "ani_.fbx";
                //自动添加动画片段, thx infinitedev:https://forum.unity.com/threads/how-can-i-get-all-the-animation-clips-imported-from-an-fbx-solved.431669/
                Object[] assetRepresentationsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(animationObjectPath);
                if (assetRepresentationsAtPath != null)
                {
                    foreach (Object assetRepresentation in assetRepresentationsAtPath)
                    {
                        AnimationClip animationClip = assetRepresentation as AnimationClip;
                        if (animationClip != null)
                        {
                            ac.AddMotion(animationClip);
                        }
                    }
                }
                Debug.Log(nameof(BoxheadModelImporter) + $" create AnimatorController in folder:{folderPath}, add {ac.animationClips.Length} clips");
            }

            //添加动画状态机:
            Animator animator = gameObject.AddComponent<Animator>();
            //绑定运行时动画控制器
            animator.runtimeAnimatorController = ac;
            //设置特定动画的播放速度speed and loop:
            //obsoleted Ref(japoilski, Mar 2, 2015):https://forum.unity.com/threads/mecanim-change-animation-speed-of-specific-animation-or-layers.160395/page-2
            //current Ref:https://docs.unity3d.com/ScriptReference/Animator.GetCurrentAnimatorStateInfo.html
            //TODO...
            Debug.Log($"{nameof(BoxheadModelImporter)} add component Animator.");

            //改变默认shader:
            Transform box001 = gameObject.transform.Find("Box001");
            if (box001)
            {
                Renderer renderer = box001.GetComponent<Renderer>();
                if (renderer)
                {
                    renderer.sharedMaterial.shader = Shader.Find("BoxheadWorld/Diffuse");
                    Debug.Log(nameof(BoxheadModelImporter) + " change sharedMaterial.shader to BoxheadWorld/Diffuse.");
                }
            }

            //!!!不要尝试在该函数中创建prefab!!!
            //以下是官方文档的警告:
            //Any references to game objects or meshes will become invalid after the import has been completed. 
            //Thus it is not possible to create a new Prefab in a different file from OnPostprocessModel that references meshes in the imported fbx file.
            //应该参考:https://docs.unity3d.com/ScriptReference/PrefabUtility.html
            //制作一个单独的类方便生成Prefab
        }
    }

    private void OnPreprocessAnimation()
    {
        //ModelImporter animImporter = assetImporter as ModelImporter;
        //animImporter.clipAnimations = animImporter.defaultClipAnimations;
        //foreach (var item in animImporter.clipAnimations)
        //{
        //    item.loop = true;
        //}
    }
}