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
        
        DressVariant dress = itemDetails.dress;
        HairVariant hair = itemDetails.hair;
        ShoesVariant shoes = itemDetails.shoes;
        PurseVariant purse = itemDetails.purse;
        WatchVariant watch = itemDetails.watch;

        if (dress != DressVariant.None) {
            SetDeactivateListComponents(dressList);
            dressList[(int)dress - 1].gameObject.SetActive(true);
        }
        else if (hair != HairVariant.None) {
            SetDeactivateListComponents(hairList);
            hairList[(int)hair - 1].gameObject.SetActive(true);
        }
        else if (shoes != ShoesVariant.None) {
            SetDeactivateListComponents(shoesList);
            shoesList[(int)shoes - 1].gameObject.SetActive(true);
        }
        else if (purse != PurseVariant.None) {
            SetDeactivateListComponents(purseList);
            purseList[(int)purse - 1].gameObject.SetActive(true);
        }
        else if (watch != WatchVariant.None) {
            SetDeactivateListComponents(watchList);
            watchList[(int)watch - 1].gameObject.SetActive(true);
        }
        else switch (itemDetails.type)
        {
            case CollectibleType.Necklace: //only 1 variant
                necklaceList[0].gameObject.SetActive(true);
                break;
            case CollectibleType.Ring: //only 1 variant
                ringList[0].gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
