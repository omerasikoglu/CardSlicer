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

    [SerializeField] private bool hasExit;
    [SerializeField, EnableIf("hasExit")] private GameObject exit;
    private Transform womanTransform;

    private void Awake()
    {
        womanTransform = womanTransform != null ? womanTransform : FindObjectOfType<WomanController>().transform;
        if(!hasExit) Destroy(exit);
    }

    public int GetCollectibleMoney()
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



    private void OnTriggerEnter(Collider collision)
    {
        WomanController womanController = collision.GetComponentInParent<WomanController>();
        if (womanController != null)
        {
            womanController.EnableWomanPart(collectibleType);

            Transform woman = womanController.GetComponent<Transform>();
            woman.DOScaleY(woman.localScale.y + 0.5f, 1f).SetEase(Ease.InBounce);
            Destroy(gameObject);
        }
    }



    public async void SetCollectibleTasks()
    {
        if(hasExit) Destroy(exit);

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
