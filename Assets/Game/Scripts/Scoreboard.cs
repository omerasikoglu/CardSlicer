using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

[Serializable]
public enum IncreaseStyle {
    LeftToRight, BottomToUp,
}
public class Scoreboard : Singleton<Scoreboard> {
    [SerializeField, BoxGroup("[Transforms]")] private Transform collectibleRoot;
    [SerializeField, BoxGroup("[Transforms]")] private Transform pointerTransform;
    //[SerializeField, BoxGroup("[Meshes]")] private MeshRenderer scoreboardMesh, pointerMesh;

    [SerializeField] private IncreaseStyle increaseStyle;
    [SerializeField] private int acquiredCollectibleAmount = 10;
    [SerializeField] private float reachTime = 5f;



    private float finalValue; // complete amount, normalized

    private int TotalCollectibleAmount => collectibleRoot.childCount;
    //private Vector2 scoreboardSize => scoreboardMesh.bounds.size;


    private void Awake() {
        SetFirstNormalizedPointerPosition();

        StartScoreboard();
    }

    private void StartScoreboard() {
        finalValue = Mathf.InverseLerp(0, TotalCollectibleAmount, acquiredCollectibleAmount);

        pointerTransform.DOLocalMove(SetFinalNormalizedPointerPosition(), 5f).SetEase(Ease.OutSine);
    }

    private void SetFirstNormalizedPointerPosition() {

        pointerTransform.localPosition = .5f * increaseStyle switch
        {
            IncreaseStyle.LeftToRight => Vector3.up,
            IncreaseStyle.BottomToUp => Vector3.right,
            _ => Vector3.zero
        };
    }
    private Vector3 SetFinalNormalizedPointerPosition() {
        return increaseStyle switch
        {
            IncreaseStyle.LeftToRight => new Vector3(finalValue, pointerTransform.localPosition.y),
            IncreaseStyle.BottomToUp => new Vector3(pointerTransform.localPosition.x, finalValue),
            _ => Vector3.zero
        };
    }
}
