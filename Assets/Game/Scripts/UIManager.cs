using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Transform scoreCircleUI;

    [SerializeField] private float totalCollectibleAmount, increaseAmount, lerpSpeed = 2f;

    private Image scoreCircleImage;

    private void Awake()
    {
        scoreCircleImage = scoreCircleUI.GetComponent<Image>();
        scoreCircleImage.fillAmount = 0f;

        increaseAmount = (1 / totalCollectibleAmount); // =1/360 = 0.0027f
    }

    [Button]
    private void CollectibleCollected()
    {
        scoreCircleImage.fillAmount += increaseAmount;
    }

}
