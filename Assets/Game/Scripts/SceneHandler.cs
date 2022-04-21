using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {
    [SerializeField, ReadOnly] private int currentLevel;
    [SerializeField] private bool isAutoLoad;
    [SerializeField] private int nonLevelSceneCount = 2;

    private int TotalLevelCount => SceneManager.sceneCountInBuildSettings - nonLevelSceneCount;
    //1 for main, 1 for environment. all others level scene

    private void Awake() {
        currentLevel = 1;
        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
        if (!isAutoLoad) return;
        SceneLoadCheck();
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
    public void LoadNextLevel() { //from ButtonUI

        UnloadThisLevel();

        currentLevel -= currentLevel + 1 > TotalLevelCount ? TotalLevelCount : 0;  //when reach the max level
        currentLevel++;

        LoadCurrentLevel();

        GameManager.Instance.ChangeState(GameState.TapToPlay);
    }
    [Button]
    public void ReloadThisLevel() { //from ButtonUI

        UnloadThisLevel();

        LoadCurrentLevel();
    }

    private void UnloadThisLevel() {
        SceneManager.UnloadSceneAsync(currentLevel + nonLevelSceneCount - 1);
    }
    private void LoadCurrentLevel() {
        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
        SceneManager.LoadSceneAsync(currentLevel + nonLevelSceneCount - 1, LoadSceneMode.Additive);
    }
}
