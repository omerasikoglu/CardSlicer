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
public class Scoreboard : MonoBehaviour {

    [SerializeField, BoxGroup("[Transforms]")] private Transform pointerTransform;

    [SerializeField] private MeshRenderer pointerRenderer,scoreboardRenderer;

    [SerializeField] private IncreaseStyle increaseStyle;

    [SerializeField] private float pointerReachTime = 5f;

    private float finalValue; // complete amount, normalized

    private void Awake() {

        SetFirstNormalizedPointerPosition();
        Debug.Log($"<Color=red>{pointerRenderer.bounds.size}</Color>");

    }

    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;


    private void GameManager_OnStateChanged(GameState gameState) {
        if (gameState != GameState.Win) return;
        TriggerTheScoreboard();
    }

    [Button]
    public void TriggerTheScoreboard() {

        finalValue = Mathf.InverseLerp(
            0, Collectibles.Instance.GetCollectibleCount, PlayerPrefs.GetInt(StringData.PREF_COLLECTED));

        pointerTransform.DOLocalMove(SetFinalNormalizedPointerPosition(), pointerReachTime).SetEase(Ease.OutSine);
    }

    private void SetFirstNormalizedPointerPosition() {

        pointerTransform.localPosition = .5f * increaseStyle switch
        {
            IncreaseStyle.LeftToRight => Vector3.up,
            IncreaseStyle.BottomToUp => Vector3.right,
            _ => Vector3.zero
        };

        pointerRenderer.transform.position -= new Vector3(
            pointerRenderer.bounds.extents.x, pointerRenderer.bounds.extents.y);
    }
    private Vector3 SetFinalNormalizedPointerPosition() {
        return increaseStyle switch
        {
            IncreaseStyle.LeftToRight => new Vector3(finalValue, pointerTransform.localPosition.y),
            IncreaseStyle.BottomToUp => new Vector3(pointerTransform.localPosition.x, finalValue),
            _ => Vector3.zero
        };
    }

    public float GetScoreboardHeight()
    {
        return scoreboardRenderer.bounds.size.y;
    }
}
