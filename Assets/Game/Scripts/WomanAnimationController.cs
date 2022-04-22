using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using NaughtyAttributes;

public class WomanAnimationController : Singleton<WomanAnimationController> {
    [SerializeField] private Animator womanAnimator;

    private void Awake() {
        womanAnimator = womanAnimator != null ? womanAnimator : GetComponent<Animator>();
    }
    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;
    private void GameManager_OnStateChanged(GameState gameState) {
        PlayAnimation(gameState);
    }
    private void PlayAnimation(GameState gameState) {
        womanAnimator.SetTrigger(gameState switch
        {
            GameState.TapToPlay => StringData.IDLE,
            GameState.Run => StringData.CAT_WALK,
            GameState.Win => StringData.HAPPY,
            GameState.Lose => StringData.CRY,
            _ => StringData.IDLE,
        })
            ;

    }
    //TODO: USE THEM WITH 1 LIST
   
    
    [Button]
    public void PlayKiss() {
        womanAnimator.SetTrigger(StringData.KISS);
    }

    [Button]
    public void PlaySpin() {
        womanAnimator.SetTrigger(StringData.SPIN);
    }
    [Button]
    public void PlayIdle() {
        womanAnimator.SetTrigger(StringData.IDLE);
    }


}
