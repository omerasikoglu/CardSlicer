using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

[Serializable]
public enum GameState {
    TapToPlay = 1,
    Run = 2,
    Win = 3,
    Lose = 4
}
[DefaultExecutionOrder(-2)]
public class GameManager : Singleton<GameManager> {
    public static event Action<GameState> OnStateChanged;
    private GameState activeState;

    private void Awake() {
        InitPlayerPrefs();
        
    }

    private static void InitPlayerPrefs() {
        PlayerPrefs.SetInt(StringData.PREF_MONEY, 0);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
    }

    private void Start()
    {
        ChangeState(GameState.TapToPlay);
    }
    public void ChangeState(GameState newState) {
        if (activeState == newState) return;

        activeState = newState;

        switch (newState)
        {
            case GameState.TapToPlay: TapToPlay(); break;
            case GameState.Run: Run(); break;
            case GameState.Win: WinGame(); break;
            case GameState.Lose: LoseGame(); break;
            default: break;
        }

        OnStateChanged?.Invoke(newState);
    }

    private void TapToPlay() {
        UIManager.Instance.ShowUI(GameUI.TapToPlay);
    }
    private void Run() {
        Time.timeScale = 1f;
    }
    private void WinGame() {
        UIManager.Instance.ShowUI(GameUI.Win);
        //character stop woman fail anim activate
    }
    private void LoseGame() {

    }

    public void SetRunningState()
    {
        ChangeState(GameState.Run);
    }

    public void SetWinningState()
    {
        ChangeState(GameState.Win);
    }
}
