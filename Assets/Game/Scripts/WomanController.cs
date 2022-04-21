using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class WomanController : Model {


    //Have at least 1 default value
    [SerializeField] private List<Transform> dressList;
    [SerializeField] private List<Transform> hairList;

    //could be empty
    [SerializeField] private List<Transform> shoesList;
    [SerializeField] private List<Transform> purseList;
    [SerializeField] private List<Transform> watchList;
    [SerializeField] private List<Transform> necklaceList;
    [SerializeField] private List<Transform> ringList;

    private void Awake() {

        InitItems();
    }

    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;

    private void GameManager_OnStateChanged(GameState obj) {
        switch (obj) {
            case GameState.TapToPlay: SetMovementSpeed(0); break;
            case GameState.Run: SetMovementSpeed(2); break;
            case GameState.Win:
                SetMovementSpeed(0);
                break;
            case GameState.Lose:
                //PlayLoseFX();
                SetMovementSpeed(0);
                break;
            default: break;
        }
    }

    private void InitItems() {

        List<List<Transform>> listContainer = new List<List<Transform>> {
            watchList,purseList,necklaceList,ringList,dressList,hairList,shoesList
        };
        foreach (List<Transform> list in listContainer) {

            int i = 0;
            while (i < list.Count) {
                list[i].gameObject.SetActive(false);
                i++;
            }
        }

        dressList[0].gameObject.SetActive(true);
        hairList[0].gameObject.SetActive(true);
        shoesList[0].gameObject.SetActive(true);
    }

    public void SetActiveWomanPart(ItemDetails itemDetails) {

        //TODO: Make It SOLID
        
        DressVariant dressVariant = itemDetails.dressVariant;
        HairVariant hairVariant = itemDetails.hairVariant;
        ShoesVariant shoesVariant = itemDetails.shoesVariant;
        PurseVariant purseVariant = itemDetails.purseVariant;
        WatchVariant watchVariant = itemDetails.watchVariant;

        if (dressVariant != 0)
        {
            SetDeactivateListComponents(dressList);
            dressList[(int)dressVariant - 1].gameObject.SetActive(true);
        }
        else if (hairVariant != 0) {
            SetDeactivateListComponents(hairList);
            hairList[(int)hairVariant - 1].gameObject.SetActive(true);
        }
        else if (shoesVariant != 0) {
            SetDeactivateListComponents(shoesList);
            shoesList[(int)shoesVariant - 1].gameObject.SetActive(true);
        }
        else if (purseVariant != 0) {
            SetDeactivateListComponents(purseList);
            purseList[(int)purseVariant - 1].gameObject.SetActive(true);
        }
        else if (watchVariant != 0) {
            SetDeactivateListComponents(watchList);
            watchList[(int)watchVariant - 1].gameObject.SetActive(true);
        }
        else switch (itemDetails.type)
        {
            case CollectibleType.Necklace: //only 1 variant
                necklaceList[0].gameObject.SetActive(true);
                break;
            case CollectibleType.Ring: //only 1 variant
                ringList[0].gameObject.SetActive(true);
                break;
            default: break;
        }

        WomanAnimationController.Instance.PlaySpin();
    }

    private void SetDeactivateListComponents(List<Transform> list) {
        foreach (Transform st in list) {
            st.gameObject.SetActive(false);
        }

    }


    private static void CheckCatWalk() {
        if (PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS, 0) >= 2) {
            WomanAnimationController.Instance.PlayCry();
        }
        else {
            WomanAnimationController.Instance.PlayCatWalk();
        }
    }
}
