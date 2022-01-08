using UnityEngine;

public class Bambo : PlayerBase
{
    public string Name;

    protected override void StartPlayer()
    {
        InitPlayerBase(LocalPlayerID.Player1, 100, ActorDirection.None, 5, "Prefabs/" + Name, 3f, 2.5f, 4f, 2.5f);
    }

    protected override void UpdatePlayer(float deltaTime)
    {
        if (IsMoving)
        {
            AnimatorPlaySafe("move_walk");
            //AnimatorPlaySafe("double_walk");
        }
        else
        {
            AnimatorPlaySafe("idle_rocket_walk");
            //AnimatorPlaySafe("idle_double_walk");
        }
        Debug.Log(CurSpeed);
    }
}