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

    public PhysicsWorldManager _PhysicsWorldManager;

    public EGameState GameState;

    public Player[] Players;
    public GameObject[] PlayerController; //?? 규태 : 서버 통신으로 변경하면 삭제한다.

    public int GamePlayerCount;
    public int GameTurnNumber;

    public float GameTurnLimitTime;
    public float GameTurnTime;

    //??
    public Text FrameText;
    private float FrameDeltaTime;

    private void Start()
    {
        InitializeGameManager();

        //?? 규태 : 임시
        Application.targetFrameRate = 60;
        StartGame();
    }
    

    private void Update()
    {
        UpdateGameManager();
        CheckTurnOver();

        UpdateDebugUI();
    }

    //?? 규태 : 우선 고정
    private void InitializeGameManager()
    {
        _PhysicsWorldManager = GameObject.Find("PhysicsWorldManager").GetComponent<PhysicsWorldManager>();

        Players = new Player[2];
        PlayerController = new GameObject[2];
        for (int i = 0; i < 2; i++)
        {
            Players[i] = GameObject.Find("Player_" + i).GetComponent<Player>();
            PlayerController[i] = GameObject.Find("Players").transform.Find("PlayerController_" + i).gameObject;
        }

        // 게임 정보
        GameState = EGameState.GAME_STATE_PAUSE;

        GamePlayerCount = 2;
        GameTurnNumber = 0;

        GameTurnLimitTime = 10000.0f;
    }

    public void StartGame()
    {
        GameState = EGameState.GAME_STATE_PLAYING;

        GameTurnNumber = 0;
        GameTurnTime = 0.0f;

        Players[GameTurnNumber].PlayerState = Player.EPlayerState.PLAYER_STATE_READY;
        PlayerController[GameTurnNumber].SetActive(true);
    }

    /*
     *  UPDATE
     */
    private void UpdateGameManager()
    {
        if (GameState != EGameState.GAME_STATE_PLAYING)
            return;

        GameTurnTime += Time.deltaTime;
    }

    private void CheckTurnOver()
    {
        if (GameTurnTime >= GameTurnLimitTime || 
            (GameState == EGameState.GAME_STATE_THROWING && _PhysicsWorldManager.GetObservedObjectCount() == 0))
        {
            SetGameTurnOver();
        }
    }

    /*
     * 
     */
    public void SetGameStatePlaying()
    {
        GameState = EGameState.GAME_STATE_PLAYING;
    }

    public void SetGameStateThrowing()
    {
        GameState = EGameState.GAME_STATE_THROWING;
    }

    public void SetGameStatePause()
    {
        GameState = EGameState.GAME_STATE_PAUSE;
    }

    public void SetGameStateOver()
    {
        GameState = EGameState.GAME_STATE_OVER;
    }

    public void SetGameTurnOver()
    {
        // 턴을 가지고 있던 유저를 disable
        Players[GameTurnNumber].SetPlayerStateIdle();
        PlayerController[GameTurnNumber].SetActive(false);

        // 턴을 다음 유저에게 넘긴다.
        GameTurnNumber++;
        if (GameTurnNumber >= GamePlayerCount)
        {
            GameTurnNumber = 0;
        }

        Players[GameTurnNumber].SetPlayerStateReady();
        PlayerController[GameTurnNumber].SetActive(true);


        // 턴을 초기화 시키고 게임을 다시 진행한다.
        GameTurnTime = 0.0f;

        SetGameStatePlaying();
    }

    //?? 규태 : 옮기기
    public void SceneMove(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateDebugUI()
    {
        FrameDeltaTime += (Time.deltaTime - FrameDeltaTime) * 0.1f;
        float fps = 1.0f / FrameDeltaTime;
        FrameText.text = fps.ToString();
    }
}
