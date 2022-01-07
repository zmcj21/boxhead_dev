using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Bambo : PlayerBase
{
    protected override void StartPlayer()
    {
        InitPlayerBase(LocalPlayerID.Player1, 100, ActorDirection.None, 5, "Prefabs/bambo", 3f, 2.5f, 4f, 2.5f);
    }

    protected override void UpdatePlayer(float deltaTime)
    {
        if (IsMoving)
        {
            //AnimatorPlaySafe("move_walk");
        }
        else
        {
            //AnimatorPlaySafe("idle_rocket_walk");
        }
        Debug.Log(CurSpeed);
    }
}