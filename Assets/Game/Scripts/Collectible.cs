using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using NaughtyAttributes;

[Serializable]
public enum CollectibleType
{
    Money = 1,
    Purse = 2,
    Dress = 3,
    Shoes = 4,
    Fur = 5,
    Hair = 6,
    Accessory = 7,
    Car = 8,
}
public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleType collectibleType;
    public CollectibleType GetCollectionType() => collectibleType;

    [SerializeField] private bool hasExit, isFallingDown;
    [SerializeField, EnableIf("hasExit")] private GameObject exit;


    private Transform womanTransform, alive, broken;

    private void Awake()
    {
        Init();
        if (!hasExit) Destroy(exit);
    }

    private void Init()
    {
        alive = GetComponentInChildren<Transform>().Find(StringData.ALIVE);
        broken = GetComponentInChildren<Transform>().Find(StringData.BROKEN);
        womanTransform = womanTransform != null ? womanTransform : FindObjectOfType<WomanController>().transform;
    }

    public int GetMoneyAmountOfCollectible()
    {
        return collectibleType switch
        {
            (CollectibleType)1 => 100,
            (CollectibleType)2 => -150,
            (CollectibleType)3 => -200,
            (CollectibleType)4 => -100,
            (CollectibleType)5 => -250,
            (CollectibleType)6 => -50,
            (CollectibleType)7 => -150,
            (CollectibleType)8 => -400,
            _ => 0,
        };
    }

    public async void PlayCollectibleTasks()
    {
        if (hasExit) Destroy(exit);

        if (isFallingDown) //Breaking Collectible
        {
            alive.gameObject.SetActive(false);
            broken.gameObject.SetActive(true);
            StartCoroutine(UtilsClass.Wait(() => { Destroy(transform.parent.gameObject); }, 1f));
            return;
        }

        var position = transform.position;
        Vector3 womanPosDelta = new Vector3(0f, 0f, 2.5f); //womanTransform's pos after 1 sec delay

        List<Task> taskList = new List<Task>
        {
            transform.DOMoveZ(position.z + 5f, .7f).AsyncWaitForCompletion(),
            transform.DOMoveY(position.y + 2f, .7f).AsyncWaitForCompletion()
        };
        await Task.WhenAll(taskList);
        taskList.Add(transform.DOScale(Vector3.one * 1.1f, .3f).AsyncWaitForCompletion());
        taskList.Add(transform.DORotate(new Vector3(0f, 90f, 0f), 1f).AsyncWaitForCompletion());
        await Task.WhenAll(taskList);
        taskList.Add(transform.DOMove(womanTransform.position + womanPosDelta, 1f).AsyncWaitForCompletion());
    }

}
