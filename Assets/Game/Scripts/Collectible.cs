using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectible : MonoBehaviour
{
    [SerializeField] private Transform woman;
    private Vector3 womanPosDelta = new Vector3(0f, 0f, 2.5f);
    private void OnTriggerEnter(Collider collision)
    {
        PlayerController player = collision.GetComponentInParent<PlayerController>();
        WomanController womanController = collision.GetComponentInParent<WomanController>();

        if (player != null)
        {
            this.transform.DOLocalMove(woman.transform.position + womanPosDelta, 1f);
        }

        if (womanController != null)
        {
            Debug.Log("woman");
        }
    }
}
