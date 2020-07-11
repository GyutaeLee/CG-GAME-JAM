using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public WorldSetter.CubeAreaEnum PlayerCubeAreaEnum;

    public EGameState GameState;

    public Player[] Players;
    public GameObject[] PlayerController; //?? 규태 : 서버 통신으로 변경하면 삭제한다.

    public int GamePlayerCount;
    public int GameTurnNumber;

    public float GameTurnLimitTime;
    public float GameTurnTime;

    private void Start()
    {
        InitializeGameManager();

        //?? 규태 : 임시
        StartGame();
    }

    private void Update()
    {
        UpdateGameManager();
        CheckTurnOver();
    }

    //?? 규태 : 우선 고정
    private void InitializeGameManager()
    {
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
        if (GameTurnTime >= GameTurnLimitTime)
        {
            SetGameTurnOver();
        }
    }

    /*
     * 
     */
    public void SetGameStateThrowing()
    {
        GameState = EGameState.GAME_STATE_THROWING;
    }

    public void SetGameTurnOver()
    {
        Players[GameTurnNumber].SetPlayerIdle();

        PlayerController[GameTurnNumber].SetActive(false);

        GameTurnNumber++;
        if (GameTurnNumber >= GamePlayerCount)
        {
            GameTurnNumber = 0;
        }

        GameTurnTime = 0.0f;

        Players[GameTurnNumber].PlayerState = Player.EPlayerState.PLAYER_STATE_READY;
        PlayerController[GameTurnNumber].SetActive(true);

        GameState = EGameState.GAME_STATE_PLAYING;
    }
}
