using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {
    [SerializeField, BoxGroup(StringData.OPTIONS)] private bool isAutoLoad;
    [SerializeField, BoxGroup("[Readonly]"), ReadOnly] private int currentLevel, previousLevel, nextLevel;
    private readonly int nonLevelSceneCount = 2;

    private int TotalLevelCount => SceneManager.sceneCountInBuildSettings - nonLevelSceneCount;
    //1 for main, 1 for environment. all others level scene

    private void Awake() {
        Init();

        if (!isAutoLoad) return;

        CheckEssentialScenesIsLoaded();
    }
    private void Init() {
        previousLevel = 0;
        currentLevel = 1;
        nextLevel = currentLevel + 1;

        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
    }
    private void CheckEssentialScenesIsLoaded() {

        //load main scene and environments
        for (int levelIndex = 0; levelIndex < nonLevelSceneCount; levelIndex++) {
            if (SceneManager.GetActiveScene().buildIndex != levelIndex) {
                SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
            }
        }

        //check any level is loaded
        bool isLoad = false;
        for (int levelIndex = 0; levelIndex < TotalLevelCount; levelIndex++) { //load 1st level when there is no level
            if (SceneManager.GetActiveScene().buildIndex == levelIndex + nonLevelSceneCount)
                isLoad = true;
        }
        if (!isLoad) SceneManager.LoadSceneAsync(nonLevelSceneCount, LoadSceneMode.Additive);
    }

    #region State
    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;
    private void GameManager_OnStateChanged(GameState gameState) {
        switch (gameState) {
            case GameState.Win: PreLoadNextLevel(); break;
            case GameState.Lose: PreLoadThisLevel(); break;
        }
    }
    private void PreLoadThisLevel() {
        SceneManager.LoadSceneAsync(GetCurrentLevelSceneIndex(), LoadSceneMode.Additive);
    }
    private void PreLoadNextLevel() {

        SceneManager.LoadSceneAsync(GetNextLevelSceneIndex(), LoadSceneMode.Additive);
    }
    #endregion

    #region from ButtonUI
    [Button]
    public void LoadNextLevelWhenClicked() { // unload old level scene. set active new one.

        previousLevel = currentLevel;
        SceneManager.UnloadSceneAsync(GetPreviousLevelSceneIndex());

        UpdateCounter(ref currentLevel); UpdateCounter(ref nextLevel);

        void UpdateCounter(ref int counter) { // total level is reached?
            counter += counter + 1 > TotalLevelCount ? -TotalLevelCount + 1 : 1;
        }

        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);

        GameManager.Instance.ChangeState(GameState.TapToPlay);
    }
    [Button]
    public void ReloadThisLevelWhenClicked() {
        UnloadThisLevel();
        GameManager.Instance.ChangeState(GameState.TapToPlay);
    }
    private void UnloadThisLevel() {
        SceneManager.UnloadSceneAsync(GetCurrentLevelSceneIndex());
    }
    #endregion

    #region GetLevelIndexes
    private int GetPreviousLevelSceneIndex() {
        return previousLevel + nonLevelSceneCount - 1;
    }
    private int GetNextLevelSceneIndex() {
        return nextLevel + nonLevelSceneCount - 1;
    }
    private int GetCurrentLevelSceneIndex() {
        return currentLevel + nonLevelSceneCount - 1;
    }
    #endregion

}
