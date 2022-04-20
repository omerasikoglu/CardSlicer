using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using NaughtyAttributes;

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
    [SerializeField] private ParticleSystem healUpFX;

    public ItemDetails GetItemDetails() => itemDetails;
    public bool IsPlayerTouchIt => exit == null;
    private Transform alive, broken;

    private void Awake() {
        Init();
        if (!hasExit) Destroy(exit);
        VerticalVolplane();
    }

    private void Init() {
        alive = GetComponentInChildren<Transform>().Find(StringData.ALIVE);
        broken = GetComponentInChildren<Transform>().Find(StringData.BROKEN);
    }
    private void VerticalVolplane() { //süzülme
        float i = UnityEngine.Random.Range(1f, 2f);
        transform.GetComponentInParent<Rigidbody>().transform.DOMoveY(transform.position.y + .2f, i)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void PlayCollectibleTasks(Vector3 womanPos) {
        if (hasExit) Destroy(exit);
        var position = transform.position;
        Vector3 womanPosDelta = new Vector3(0f, 2f, 2.5f); //womanTransform's pos after 1 sec delay

        float height = UnityEngine.Random.Range(1.5f, 2f), ascendTime = UnityEngine.Random.Range(.3f, .4f);

        transform.DOMoveZ(position.z + 5f, ascendTime);
        transform.DOMoveY(position.y + height, ascendTime).OnComplete(() =>
        {
            transform.DOScale(transform.localScale * 2f, ascendTime);
            transform.DORotate(new Vector3(0f, 90f, 0f), ascendTime).OnComplete(() =>
            {
                if (transform != null)
                    transform.DOMove(womanPos + womanPosDelta, 1f);
            });
        });
    }
    public void PlayHealUpFX() {
        if (healUpFX != null) healUpFX.Play();
    }

    private void OnTriggerEnter(Collider collision) {

        PlayerController player = collision.attachedRigidbody.GetComponent<PlayerController>();

        if (player != null && AffordMoney()) {

            player.ItemCollected(this);
            PlayHealUpFX();
            PlayCollectibleTasks(player.GetWomanPosition());
        }

        WomanController woman = collision.GetComponent<WomanController>();
        if (woman != null && IsPlayerTouchIt) {

            woman.SetActiveWomanPart(itemDetails);
            int collected = PlayerPrefs.GetInt(StringData.PREF_COLLECTED);
            PlayerPrefs.SetInt(StringData.PREF_COLLECTED, ++collected);
            Destroy(gameObject);
        }
    }

    private bool AffordMoney() {
        int itemMoneyAmount = GetItemDetails().money;
        int currentMoney = PlayerPrefs.GetInt(StringData.PREF_MONEY, 0);

        if (currentMoney + itemMoneyAmount < 0) {
            return false;
        }

        currentMoney += itemMoneyAmount;
        PlayerPrefs.SetInt(StringData.PREF_MONEY, currentMoney);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);

        return true;
    }
}
