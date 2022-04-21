using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldActivator : Singleton<WorldActivator> {
    private void Awake() {
        SetDisactiveChildren();

        //StartCoroutine(UtilsClass.Wait(SetActiveChildren, 1f));
    }

    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;

    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;

    private void GameManager_OnStateChanged(GameState obj)
    {
        switch (obj)
        {
            case GameState.TapToPlay:
                SetActiveChildren();
                break;
        }
    }

    public void SetDisactiveChildren() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void SetActiveChildren() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }
}
