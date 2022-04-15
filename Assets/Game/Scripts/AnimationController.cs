using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AnimationController : Singleton<AnimationController>
{
    [SerializeField] private Animator womanAnimator;

    private void Awake()
    {
        GameManager.OnStateChanged += GameManager_OnStateChanged;
        womanAnimator = womanAnimator != null ? womanAnimator : GetComponent<Animator>();
    }

    private void GameManager_OnStateChanged(GameState obj) {
        switch (obj)
        {
            case GameState.TapToPlay:
                PlayIdle();
                break;
            case GameState.Run:
                PlayCatWalk();
                break;
            case GameState.WinGame:
                PlayHappy();
                break;
            case GameState.LoseGame:
                PlayCry();
                break;
            default: break;
        }
    }

    [Button]
    public void PlayIdle()
    {
        womanAnimator.SetTrigger(StringData.IDLE);
    }
    [Button]
    public void PlayCry()
    {
        womanAnimator.SetTrigger(StringData.CRY);
    }
    [Button]
    public void PlayKiss()
    {
        womanAnimator.SetTrigger(StringData.KISS);
    }
    [Button]
    public void PlayHappy()
    {
        womanAnimator.SetTrigger(StringData.HAPPY);
    }
    [Button]
    public void PlayCatWalk()
    {
        womanAnimator.SetTrigger(StringData.CAT_WALK);
    }
}
