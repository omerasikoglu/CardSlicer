using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine.AI;

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
[Serializable] public enum WatchVariant { Watch1 = 1, Watch2 = 2, Watch3 = 3, None=4 }
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
            (CollectibleType)4 => -150,
            (CollectibleType)5 => -50,
            (CollectibleType)6 => -100,
            (CollectibleType)7 => -250,
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
    private Transform womanTransform, alive, broken;

    private void Awake() {
        Init();
        if (!hasExit) Destroy(exit);
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

        List<Task> taskList = new List<Task>
        {
            transform.DOMoveZ(position.z + 5f, .7f).AsyncWaitForCompletion(),
            transform.DOMoveY(position.y + 2f, .7f).AsyncWaitForCompletion()
        };
        await Task.WhenAll(taskList);
        taskList.Add(transform.DOScale(Vector3.one * 1.5f, .3f).AsyncWaitForCompletion());
        taskList.Add(transform.DORotate(new Vector3(0f, 90f, 0f), 1f).AsyncWaitForCompletion());
        await Task.WhenAll(taskList);
        taskList.Add(transform.DOMove(womanTransform.position + womanPosDelta, 1f).AsyncWaitForCompletion());
    }

}
