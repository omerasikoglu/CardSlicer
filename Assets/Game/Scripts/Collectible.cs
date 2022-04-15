using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using NaughtyAttributes;
using Random = System.Random;

#region ItemType
[Serializable]
public enum CollectibleType {
    //no feature
    Money = 1,
    //multi sample
    Dress = 2, Hair = 3, Shoes = 4, Purse = 5, Watch = 6,
    //one sample
    Ring = 7, Necklace = 8,
}
[Serializable] public enum DressVariant { Dress1 = 1, Dress2 = 2, Dress3 = 3, Dress4 = 4, None = 5 }
[Serializable] public enum HairVariant { Hair1 = 1, Hair2 = 2, Hair3 = 3, None = 4 }
[Serializable] public enum ShoesVariant { Shoes1 = 1, Shoes2 = 2, Shoes3 = 3, None = 4 }
[Serializable] public enum PurseVariant { Purse1 = 1, Purse2 = 2, Purse3 = 3, None = 4 }
[Serializable] public enum WatchVariant { Watch1 = 1, Watch2 = 2, Watch3 = 3, None = 4 }
[Serializable]
public struct ItemDetails {
    public CollectibleType type;
    public int money => HowMuchYouEarn();
    [AllowNesting, ShowIf("IsDress")] public DressVariant dress;
    [AllowNesting, ShowIf("IsHair")] public HairVariant hair;
    [AllowNesting, ShowIf("IsShoes")] public ShoesVariant shoes;
    [AllowNesting, ShowIf("IsPurse")] public PurseVariant purse;
    [AllowNesting, ShowIf("IsWatch")] public WatchVariant watch;

    private bool IsDress() { return type == CollectibleType.Dress; }
    private bool IsHair() { return type == CollectibleType.Hair; }
    private bool IsShoes() { return type == CollectibleType.Shoes; }
    private bool IsPurse() { return type == CollectibleType.Purse; }
    private bool IsWatch() { return type == CollectibleType.Watch; }

    private int HowMuchYouEarn() { //could be negative
        return type switch
        {
            (CollectibleType)1 => 100,
            (CollectibleType)2 => -200,
            (CollectibleType)3 => -50,
            (CollectibleType)4 => -100,
            (CollectibleType)5 => -150,
            (CollectibleType)6 => -150,
            (CollectibleType)7 => -300,
            (CollectibleType)8 => -300,
            _ => 0,
        };
    }
};
#endregion
public class Collectible : MonoBehaviour {
    [SerializeField, BoxGroup("[Item Details]")] private ItemDetails itemDetails;
    [SerializeField, BoxGroup("[Destroy Style]")] private bool isFallingDown;
    [SerializeField, BoxGroup("[Collider]")] private bool hasExit;
    [SerializeField, BoxGroup("[Collider]"), EnableIf("hasExit")] private GameObject exit;

    public ItemDetails GetItemDetails() => itemDetails;
    public bool IsPlayerTouchIt => exit == null;
    private Transform womanTransform, alive, broken;

    private void Awake() {
        Init();
        if (!hasExit) Destroy(exit);
        VerticalVolplane();
    }

    private void VerticalVolplane() { //süzülme
        float i = UnityEngine.Random.Range(1f, 2f);
        transform.GetComponentInParent<Rigidbody>().transform.DOMoveY(transform.position.y + .2f, i)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void Init() {
        alive = GetComponentInChildren<Transform>().Find(StringData.ALIVE);
        broken = GetComponentInChildren<Transform>().Find(StringData.BROKEN);
        womanTransform = womanTransform != null ? womanTransform : FindObjectOfType<WomanController>().transform;
    }

    public async void PlayCollectibleTasks() {
        if (hasExit) Destroy(exit);

        if (isFallingDown) //Breaking Collectible
        {
            alive.gameObject.SetActive(false);
            broken.gameObject.SetActive(true);
            StartCoroutine(UtilsClass.Wait(() => { Destroy(transform.parent.gameObject); }, 1f));
            return;
        }

        var position = transform.position;
        Vector3 womanPosDelta = new Vector3(0f, 2f, 2.5f); //womanTransform's pos after 1 sec delay

        float height = UnityEngine.Random.Range(1.5f, 2f), ascendTime = UnityEngine.Random.Range(.3f, .4f);
        List<Task> taskList = new List<Task>
        {
            transform.DOMoveZ(position.z + 5f, ascendTime).AsyncWaitForCompletion(),
            transform.DOMoveY(position.y + height, ascendTime).AsyncWaitForCompletion()
        };
        await Task.WhenAll(taskList);
        taskList.Add(transform.DOScale(transform.localScale * 2f, ascendTime).AsyncWaitForCompletion());
        taskList.Add(transform.DORotate(new Vector3(0f, 90f, 0f), ascendTime).AsyncWaitForCompletion());
        await Task.WhenAll(taskList);
        if (transform != null) taskList.Add(transform.DOMove(womanTransform.position + womanPosDelta, 1f).AsyncWaitForCompletion());
    }

}
