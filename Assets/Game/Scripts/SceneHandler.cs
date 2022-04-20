using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {
    [SerializeField] private int currentLevel;
    [SerializeField] private bool isAutoLoad;
    private int TotalLevelCount => SceneManager.sceneCountInBuildSettings - 2;
    //1 for main, 1 for environment. all others level scene
    private void Start() {
        if (!isAutoLoad) return;
        SceneLoadCheck();
    }

    private void SceneLoadCheck() {
        currentLevel = 0;

        int[] exactLevelIndexs = { 0, 2 };
        foreach (var t in exactLevelIndexs) {
            if (SceneManager.GetActiveScene().buildIndex != t) {
                SceneManager.LoadSceneAsync(t, LoadSceneMode.Additive);
            }
        }

        bool isAnyLevel = false;
        for (int i = 1; i < 4; i++) { //load 1st level when there is no level
            if (SceneManager.GetActiveScene().buildIndex == i) isAnyLevel = true;
        }

        if (!isAnyLevel) SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    [Button]
    public void LoadNextLevel() { //from ButtonUI

        UnloadThisLevel();

        currentLevel += currentLevel == TotalLevelCount ? 1 - TotalLevelCount : 1;  //when reach the max level

        LoadCurrentLevel();
    }
    [Button]
    public void ReloadThisLevel() { //from ButtonUI

        UnloadThisLevel();

        LoadCurrentLevel();
    }

    private void UnloadThisLevel() {
        currentLevel = PlayerPrefs.GetInt(StringData.PREF_LEVEL);
        SceneManager.UnloadSceneAsync(currentLevel);
    }
    private void LoadCurrentLevel() {
        PlayerPrefs.SetInt(StringData.PREF_LEVEL, currentLevel);
        currentLevel = PlayerPrefs.GetInt(StringData.PREF_LEVEL);
        SceneManager.UnloadSceneAsync(currentLevel);
    }
}
