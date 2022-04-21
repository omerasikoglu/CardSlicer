using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public enum GameUI {
    TapToPlay = 1,
    InGame = 2,
    Win = 3,
    Lose = 4,
    NextLevel = 5,
}
public class UIManager : Singleton<UIManager> {

    private Transform tapToPlayUI, inGameUI, youWinUI, youLoseUI;
    private List<Transform> uiList;

    private GameUI activeUI;

    protected void Awake() {
        Init();
        DisableAllUIs();
    }
    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;
    private void GameManager_OnStateChanged(GameState gameState) {
        switch (gameState)
        {
            case GameState.TapToPlay:
                ShowUI(GameUI.TapToPlay);
                break;
            case GameState.Run:
                ShowUI(GameUI.InGame);
                break;
            case GameState.Win:
                ShowUI(GameUI.Win);
                break;
            case GameState.Lose:
                ShowUI(GameUI.Lose);
                break;
            default: break;
        }
    }

    private void Init() {
        tapToPlayUI = transform.Find("tapToPlayUI");
        inGameUI = transform.Find("inGameUI");
        youWinUI = transform.Find("youWinUI");
        youLoseUI = transform.Find("youLoseUI");
        uiList = new List<Transform>(4) { tapToPlayUI, inGameUI, youWinUI, youLoseUI };
    }

    public void ShowUI(GameUI newUI) { //only shows exact UI
        if (activeUI == newUI) return;

        activeUI = newUI;

        DisableAllUIs();
        uiList[(int)newUI - 1].gameObject.SetActive(true);
    }

    private void DisableAllUIs() {
        foreach (Transform ui in uiList)
        {
            ui.gameObject.SetActive(false);
        }
    }
    #region Test
    [Button] public void SetActiveTapToPlayUI() => uiList[1].gameObject.SetActive(true);

    [Button]
    public void SetActiveInGameUI() {
        ShowUI(GameUI.InGame);

    }
    [Button]
    public void SetActiveYouLoseUI() {
        ShowUI(GameUI.Lose);
    }
    [Button]
    public void SetActiveYouWinUI() {
        ShowUI(GameUI.Win);
    }
    #endregion
}
