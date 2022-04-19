using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Scoreboard : MonoBehaviour {
    [SerializeField] private Transform collectibleRoot;

    private int TotalCollectibleAmount => collectibleRoot.childCount;

    private MeshRenderer scoreboardRenderer => GetComponentInChildren<MeshRenderer>();

    

    private void Awake()
    {
        Debug.Log($"<color=red>{scoreboardRenderer.bounds}</color>");
        Debug.Log($"<color=green>{scoreboardRenderer.bounds.max}</color>");
        Debug.Log($"<color=blue>{scoreboardRenderer.bounds.max.y}</color>");
    }
}
