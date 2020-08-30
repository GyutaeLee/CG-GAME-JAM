using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePlayer : Player
{
    private StageGameManager m_stageGameManager;
    
    public override void InitPlayer()
    {
        base.InitPlayer();

        this.m_stageGameManager = GameObject.Find("StageGameManager").GetComponent<StageGameManager>();
    }

    public override void ThrowObject(float throwStrength)
    {
        base.ThrowObject(throwStrength);

        //?? 규태 : 현재는 계속 던질 수 있게 만들어둔다 (테스트코드)
        SetPlayerStateReady();
    }
}
