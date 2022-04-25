using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class WomanController : Model {

    //Dress, hair and shoes have at least 1 default value
    [SerializeField, Foldout("[Transforms]")]
    private List<Transform> dressList, hairList, shoesList, purseList, watchList, necklaceList, ringList;

    private List<List<Transform>> womanPieceTransformsList;

    private void Awake() => Init();

    private void Init() {

        womanPieceTransformsList = new List<List<Transform>> {
            dressList, hairList, shoesList, purseList, watchList,necklaceList,ringList
        };

        foreach (Transform piece in womanPieceTransformsList.SelectMany(o => o)) {
            piece.gameObject.SetActive(false);
        }

        //Activate first 3 bcs at least 1 dress, hair and shoes must exist
        for (int i = 0; i < 3; i++) womanPieceTransformsList[i][0].gameObject.SetActive(true);
    }

    #region States
    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;

    private void GameManager_OnStateChanged(GameState gameState) {
        SetMovementSpeed(gameState == GameState.Run ? defaultSpeed : 0);

        switch (gameState) {
            case GameState.Scoreboard:
                PlayScoreboardState();
                break;
            case GameState.Run:
                rotateWomanTween?.PlayBackwards();
                break;
            default: break;
        }
    }

    private Tween rotateWomanTween;
    private void PlayScoreboardState() {
        rotateWomanTween?.Kill();
        rotateWomanTween = transform.DORotate(new Vector3(0f, -160f, 0f), 2f).
            SetEase(Ease.InOutSine).SetAutoKill(false).
            OnComplete(() =>
        {
            WomanAnimationController.Instance.PlayIdle();
        });
    }
    #endregion

    #region Dependencies
    public void SetActiveWomanPart(ItemDetails itemDetails) { //Call from Collectible OnTrigger

        List<int> womanPieceList = new List<int> {
            (int) itemDetails.dressVariant, (int)itemDetails.hairVariant, (int)itemDetails.shoesVariant,
            (int)itemDetails.purseVariant, (int)itemDetails.watchVariant, (int)itemDetails.necklaceVariant,
            (int)itemDetails.ringVariant
        };

        for (int i = 0; i < womanPieceList.Count; i++) { //Pick exact item's true variant
            if (womanPieceList[i] == 0) continue;

            SetDeactiveListComponents(womanPieceTransformsList[i]);

            womanPieceTransformsList[i][womanPieceList[i] - 1].gameObject.SetActive(true);
        }

        WomanAnimationController.Instance.PlaySpin();
    }

    private void SetDeactiveListComponents(List<Transform> list) {
        foreach (Transform st in list) {
            st.gameObject.SetActive(false);
        }
    } 

    #endregion
}
