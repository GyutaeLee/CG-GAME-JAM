using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum EGameState
    { 
        GAME_STATE_NONE     = 0,
        
        GAME_STATE_PLAYING,
        GAME_STATE_THROWING,

        GAME_STATE_PAUSE,
        GAME_STATE_OVER,
    }

    public PhysicsWorldManager physicsWorldManager;

    public EGameState eGameState;

    public Player[] playerArray;
    public GameObject[] playerController; //?? 규태 : 서버 통신으로 변경하면 삭제한다.

    public int gamePlayerCount;
    public int gameCurrentTurn;

    public float gameTurnLimitTime;
    public float gameCurrentTurnTime;

    //?? 규태 : 프레임 체크용. debug 모드에서만 쓰자
    public Text frameText;
    private float m_frameDeltaTime;

    private void Start()
    {
        //?? 규태 : 임시
        Application.targetFrameRate = 60;

        InitGameManager();
        StartGame();
    }
    

    private void Update()
    {
        UpdateGameManager();
        CheckTurnOver();

        UpdateDebugUI();
    }

    //?? 규태 : 값들 우선으로 고정하고, 추후에 수정한다.
    private void InitGameManager()
    {
        this.physicsWorldManager = GameObject.Find("PhysicsWorldManager").GetComponent<PhysicsWorldManager>();

        this.playerArray = new Player[2];
        this.playerController = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            this.playerArray[i] = GameObject.Find("Player_" + i).GetComponent<Player>();
            this.playerController[i] = GameObject.Find("Players").transform.Find("PlayerController_" + i).gameObject;
        }

        // 게임 정보
        this.eGameState = EGameState.GAME_STATE_PAUSE;

        this.gamePlayerCount = 2;
        this.gameCurrentTurn = 0;

        this.gameTurnLimitTime = 10000.0f;
    }

    public void StartGame()
    {
        this.eGameState = EGameState.GAME_STATE_PLAYING;

        this.gameCurrentTurn = 0;
        this.gameCurrentTurnTime = 0.0f;

        this.playerArray[this.gameCurrentTurn].SetPlayerStateReady();
        this.playerController[this.gameCurrentTurn].SetActive(true);
    }

    /*
     *  UPDATE
     */
    private void UpdateGameManager()
    {
        if (this.eGameState != EGameState.GAME_STATE_PLAYING)
            return;

        this.gameCurrentTurnTime += Time.deltaTime;
    }

    private void CheckTurnOver()
    {
        if (IsTurnOver() == true)
        {
            SetGameTurnOver();
        }
    }

    private bool IsTurnOver()
    {
        bool bResult = false;

        if (this.gameCurrentTurnTime > this.gameTurnLimitTime ||
            (this.eGameState == EGameState.GAME_STATE_THROWING && physicsWorldManager.GetObservedObjectCount() == 0))
        {
            bResult = true;
        }

        return bResult;
    }

    /*
     * 
     */
    public void SetGameStatePlaying()
    {
        this.eGameState = EGameState.GAME_STATE_PLAYING;
    }

    public void SetGameStateThrowing()
    {
        this.eGameState = EGameState.GAME_STATE_THROWING;
    }

    public void SetGameStatePause()
    {
        this.eGameState = EGameState.GAME_STATE_PAUSE;
    }

    public void SetGameStateOver()
    {
        this.eGameState = EGameState.GAME_STATE_OVER;
    }

    public void SetGameTurnOver()
    {
        // 턴을 가지고 있던 유저를 disable
        this.playerArray[this.gameCurrentTurn].SetPlayerStateIdle();
        this.playerController[this.gameCurrentTurn].SetActive(false);

        // 턴을 다음 유저에게 넘긴다.
        this.gameCurrentTurn++;
        if (this.gameCurrentTurn >= this.gamePlayerCount)
        {
            this.gameCurrentTurn = 0;
        }

        this.playerArray[this.gameCurrentTurn].SetPlayerStateReady();
        this.playerController[this.gameCurrentTurn].SetActive(true);


        // 턴을 초기화 시키고 게임을 다시 진행한다.
        this.gameCurrentTurnTime = 0.0f;

        SetGameStatePlaying();
    }

    //?? 규태 : 다른 스크립트로 옮기기
    public void SceneMove(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateDebugUI()
    {
        this.m_frameDeltaTime += (Time.deltaTime - this.m_frameDeltaTime) * 0.1f;
        float fps = 1.0f / this.m_frameDeltaTime;
        this.frameText.text = fps.ToString();
    }
}
