using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageGameManager : MonoBehaviour
{
    public enum EGameState
    {
        GAME_STATE_NONE = 0,

        GAME_STATE_PLAYING,
        GAME_STATE_THROWING,

        GAME_STATE_PAUSE,
        GAME_STATE_OVER,
    }

    private EGameState m_eGameState;

    private StagePlayer m_stagePlayer;

    private float m_gameCurrentTime;

    private void Start()
    {
        //?? 규태 : 임시
        Application.targetFrameRate = 60;

        InitStageGameManager();
        StartStageGame();
        
    }

    private void Update()
    {
        UpdateStageGame();
    }

    private void InitStageGameManager()
    {
        this.m_stagePlayer = GameObject.Find("StagePlayer").GetComponent<StagePlayer>();
        this.m_stagePlayer.SetPlayerViewThirdView();
        this.m_stagePlayer.SetPlayerStateReady();

        this.m_eGameState = EGameState.GAME_STATE_PAUSE;
    }

    private void StartStageGame()
    {
        this.m_eGameState = EGameState.GAME_STATE_PLAYING;
    }

    private void UpdateStageGame()
    {
        if (this.m_eGameState == EGameState.GAME_STATE_PAUSE || this.m_eGameState == EGameState.GAME_STATE_OVER)
            return;

        this.m_gameCurrentTime += Time.deltaTime;
    }

    /*
     *  public
     */
    public void SetStageGameStatePlaying()
    {
        this.m_eGameState = EGameState.GAME_STATE_PLAYING;
    }

    public void SetStageGameStateThrowing()
    {
        this.m_eGameState = EGameState.GAME_STATE_THROWING;
    }

    public void SetStageGameStatePause()
    {
        this.m_eGameState = EGameState.GAME_STATE_PAUSE;
    }

    public void SetStageGameStateOver()
    {
        this.m_eGameState = EGameState.GAME_STATE_OVER;
    }

    //?? 규태 : 다른 스크립트로 옮기기
    public void SceneMove(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
