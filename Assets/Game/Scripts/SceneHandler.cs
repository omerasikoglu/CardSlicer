using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {
    [SerializeField, ReadOnly] private int currentLevel, previousLevel, nextLevel;
    [SerializeField] private bool isAutoLoad;
    [SerializeField] private int nonLevelSceneCount = 2;

    private int TotalLevelCount => SceneManager.sceneCountInBuildSettings - nonLevelSceneCount;
    //1 for main, 1 for environment. all others level scene

    private void Awake() {
        previousLevel = 0; currentLevel = 1; nextLevel = currentLevel + 1;

        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
        if (!isAutoLoad) return;
        SceneLoadCheck();
    }

    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;


    private void GameManager_OnStateChanged(GameState obj) {
        if (obj == GameState.Win) PreLoadNextLevel();
    }

    private void SceneLoadCheck() {

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

    [Button]
    public void PreLoadNextLevel() {

        SceneManager.LoadSceneAsync(GetNextLevelSceneIndex(), LoadSceneMode.Additive);
    }

    [Button]
    public void UnloadThisLevelWhenClicked() { // destroy old game level. setactive next level

        previousLevel = currentLevel;
        SceneManager.UnloadSceneAsync(GetPreviousLevelSceneIndex());

        UpdateCounter(ref currentLevel); UpdateCounter(ref nextLevel);

        void UpdateCounter(ref int counter)
        {
            counter += counter + 1 > TotalLevelCount ? -TotalLevelCount + 1 : 1;
        }

        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);

        GameManager.Instance.ChangeState(GameState.TapToPlay);
    }

    private int GetPreviousLevelSceneIndex() {
        return previousLevel + nonLevelSceneCount - 1;
    }
    private int GetNextLevelSceneIndex() {
        return nextLevel + nonLevelSceneCount - 1;
    }

    //#region New Level Scene Loaded When This Level Ended
    //[Button]
    //public void LoadNextLevelWhenClicked() { //from ButtonUI

    //    UnloadThisLevel();

    //    currentLevel -= currentLevel + 1 > TotalLevelCount ? TotalLevelCount : 0;  //when reach the max level
    //    currentLevel++;

    //    LoadCurrentLevel();

    //    GameManager.Instance.ChangeState(GameState.TapToPlay);
    //}
    //[Button]
    //public void ReloadThisLevelWhenClicked() { //from ButtonUI

    //    UnloadThisLevel();

    //    LoadCurrentLevel();
    //}

    //private void UnloadThisLevel() {
    //    SceneManager.UnloadSceneAsync(currentLevel + nonLevelSceneCount - 1);
    //}
    //private void LoadCurrentLevel() {
    //    PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
    //    SceneManager.LoadSceneAsync(currentLevel + nonLevelSceneCount - 1, LoadSceneMode.Additive);
    //} 
    //#endregion
}
