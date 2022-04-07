using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class WomanController : Model
{

    [SerializeField] private List<Transform> collectiblePartList;
    [SerializeField] private List<ParticleSystem> particleList; //0=>good, 1=>bad, 2=>lose
    [SerializeField] private Transform fxRoot;

    private void Awake()
    {
        DisableAllWomanParts();
    }

    private void DisableAllWomanParts()
    {
        foreach (Transform part in collectiblePartList)
        {
            part.gameObject.SetActive(false);
        }
    }

    public void EnableWomanPart(CollectibleType type)
    {
        collectiblePartList[(int)type].gameObject.SetActive(true);
        particleList[0].Play();
    }

    [Button]
    void st()
    {
    }

}
