using UnityEngine;

public class Bambo : PlayerBase
{
    public string ModelName = "bambo";

    public string ModelPrefabPath = "Prefabs";

    public bool LogCurSpeed = true;

    protected override void StartPlayer()
    {
        string prefabPath = string.Empty;
        //如果路径不包含/或\
        if (ModelPrefabPath.IndexOf('/') == -1 && ModelPrefabPath.IndexOf('\\') == -1)
        {
            prefabPath = ModelPrefabPath + "/" + ModelName;
        }
        else
        {
            prefabPath = ModelPrefabPath + ModelName;
        }
        InitPlayerBase(LocalPlayerID.Player1, 100, ActorDirection.None, 5, prefabPath, 3f, 2.5f, 4f, 2.5f);
    }

    protected override void UpdatePlayer(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //Kill();
            BeHit(10);
        }
        //Debug.Log(CurSpeed);
        Debug.Log(Health);
    }
}