using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AnimationController : Singleton<AnimationController>
{
    [SerializeField] private Animator womanAnimator;

    [Button]
    public void AnimateIdle()
    {
        womanAnimator.SetTrigger(StringData.IDLE);
    }
    [Button]
    public void AnimateCry()
    {
        womanAnimator.SetTrigger(StringData.CRY);
    }
    [Button]
    public void AnimateKiss()
    {
        womanAnimator.SetTrigger(StringData.KISS);
    }
    [Button]
    public void AnimateHappy()
    {
        womanAnimator.SetTrigger(StringData.HAPPY);
    }
    [Button]
    public void AnimateCatWalk()
    {
        womanAnimator.SetTrigger(StringData.CAT_WALK);
    }
}
