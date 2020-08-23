using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePlayer : Player
{
    public override void InitPlayer()
    {
        base.InitPlayer();

        base.m_gameManager = GameObject.Find("StageGameManager").GetComponent<GameManager>();
    }
}
