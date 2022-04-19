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

    [SerializeField] private List<ParticleSystem> particleList; //0=>good, 1=>bad, 2=>lose, 3=>hanabi

    private void Awake() {
        GameManager.OnStateChanged += GameManager_OnStateChanged;
        InitItems();
    }

    private void GameManager_OnStateChanged(GameState obj) {
        switch (obj)
        {
            case GameState.TapToPlay: SetMovementSpeed(0); break;
            case GameState.Run: SetMovementSpeed(2); break;
            case GameState.Win:
                PlayGoodFX();
                SetMovementSpeed(0);
                break;
            case GameState.Lose:
                //PlayLoseFX();
                SetMovementSpeed(0);
                break;
            default: break;
        }
    }

    private void OnTriggerEnter(Collider collision) {
        Collectible collectible = collision.GetComponentInParent<Collectible>();

        if (collectible == null) return;
        if (!collectible.IsPlayerTouchIt) return;

        SetActiveWomanPart(collectible.GetItemDetails());

        //HappyWhileWalkingTasksStart();
        //StartCoroutine(PlayHappyWhileWalking());
        WomanAnimationController.Instance.PlaySpin();


        collision.gameObject.SetActive(false);
        StartCoroutine(UtilsClass.Wait(() => { Destroy(collision.gameObject); }, 1f));
    }

    private void InitItems() {
        //TODO: make it SOLID
        foreach (Transform st in watchList)
        {
            st.gameObject.SetActive(false);
        }
        foreach (Transform st in purseList)
        {
            st.gameObject.SetActive(false);
        }
        foreach (Transform st in shoesList)
        {
            st.gameObject.SetActive(false);
        }
        foreach (Transform st in necklaceList)
        {
            st.gameObject.SetActive(false);
        }
        foreach (Transform st in ringList)
        {
            st.gameObject.SetActive(false);
        }
        //at least 1 active
        foreach (Transform st in dressList)
        {
            st.gameObject.SetActive(false);
        }
        foreach (Transform st in hairList)
        {
            st.gameObject.SetActive(false);
        }
        dressList[0].gameObject.SetActive(true);
        hairList[0].gameObject.SetActive(true);
        shoesList[0].gameObject.SetActive(true);
    }

    public void SetActiveWomanPart(ItemDetails itemDetails) {

        var dress = itemDetails.dress;
        var hair = itemDetails.hair;
        var shoes = itemDetails.shoes;
        var purse = itemDetails.purse;
        var watch = itemDetails.watch;

        //Debug.Log(itemDetails.purse); Debug.Log(itemDetails.shoes); Debug.Log(itemDetails.watch); Debug.Log(itemDetails.hair); Debug.Log(itemDetails.dress);

        if (dress != 0 && dress != DressVariant.None)
        {
            SetDeactivateListComponents(dressList);
            dressList[(int)dress - 1].gameObject.SetActive(true);
        }
        else if (hair != 0 && hair != HairVariant.None)
        {
            SetDeactivateListComponents(hairList);
            hairList[(int)hair - 1].gameObject.SetActive(true);
        }
        else if (shoes != 0 && shoes != ShoesVariant.None)
        {
            SetDeactivateListComponents(shoesList);
            shoesList[(int)shoes - 1].gameObject.SetActive(true);
        }
        else if (purse != 0 && purse != PurseVariant.None)
        {
            SetDeactivateListComponents(purseList);
            purseList[(int)purse - 1].gameObject.SetActive(true);
        }
        else if (watch != 0 && watch != WatchVariant.None)
        {
            SetDeactivateListComponents(watchList);
            watchList[(int)watch - 1].gameObject.SetActive(true);
        }
        else if (itemDetails.type == CollectibleType.Necklace)
        {//only 1 variant
            necklaceList[0].gameObject.SetActive(true);
        }
        else if (itemDetails.type == CollectibleType.Ring)
        {//only 1 variant
            ringList[0].gameObject.SetActive(true);
        }

        PlayHanabiFX();
    }

    private void SetDeactivateListComponents(List<Transform> list) {
        foreach (Transform st in list)
        {
            st.gameObject.SetActive(false);
        }

    }

    private IEnumerator PlayHappyWhileWalking() {
        
        float rotateTime = .4f;

        WomanAnimationController.Instance.PlayHappy();
        transform.DORotate(new Vector3(0, -180, 0), rotateTime);
        yield return new WaitForSeconds(rotateTime);
        transform.DOMoveY(0f, .6f);
        transform.DORotate(Vector3.zero, rotateTime);
        yield return new WaitForSeconds(rotateTime);
        CheckCatWalk();
    }
    public IEnumerator PlayTearsWhileWalking() {
        float rotateTime = .4f;

        WomanAnimationController.Instance.PlayCry();
        transform.DORotate(new Vector3(0, -180, 0), rotateTime);
        yield return new WaitForSeconds(rotateTime);
        transform.DOMoveY(0f, .6f);
        transform.DORotate(Vector3.zero, rotateTime);
        yield return new WaitForSeconds(rotateTime);

        CheckCatWalk();



    }

   
    private static void CheckCatWalk()
    {
        if (PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS, 0) >= 2)
        {
            WomanAnimationController.Instance.PlayCry();
        }
        else
        {
            WomanAnimationController.Instance.PlayCatWalk();
        }
    }

    //[Button]
    //private async Task HappyWhileWalkingTasksStart() {
    //    await PlayAnimationTask(() =>
    //    {
    //        WomanAnimationController.Instance.PlayHappy();
    //    });
    //}
    //[Button]
    //public async Task CryWhileWalkingTasksStart() {
    //    await PlayAnimationTask(() =>
    //    {
    //        WomanAnimationController.Instance.PlayCry();
    //    });
    //}
    //private async Task PlayAnimationTask(Action action) {
    //    float rotateTime = .4f;

    //    List<Task> taskList = new List<Task>
    //    {
    //        transform.DORotate(new Vector3(0,-180,0), rotateTime).AsyncWaitForCompletion()
    //    };
    //    await Task.WhenAll(taskList);
    //    action();
    //    taskList.Add(transform.DOMoveY(0f, .6f).AsyncWaitForCompletion());
    //    await Task.WhenAll(taskList);
    //    taskList.Add(transform.DORotate(Vector3.zero, rotateTime).AsyncWaitForCompletion());
    //    await Task.WhenAll(taskList);
    //    WomanAnimationController.Instance.PlayCatWalk();


    //}

    #region FX
    public void PlayGoodFX() => PlayFX(particleList[0]);
    public void PlayBadFX() => PlayFX(particleList[1]);
    public void PlayLoseFX() => PlayFX(particleList[2]);
    private void PlayHanabiFX() => PlayFX(particleList[3], 1);

    private void PlayFX(ParticleSystem particle, int isOldEmitPending = 0) {
        //isOldEmitPending => 0 stop instantly
        foreach (ParticleSystem st in particleList)
        {
            st.Stop(true, (ParticleSystemStopBehavior)isOldEmitPending);
        }
        particle.Play();
    }
    #endregion

}
