using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField] private Transform laserOrigin;

    private LineRenderer laserLine;
    [SerializeField] private float distance = 5f;
    [SerializeField, SortingLayer] private LayerMask hitLayerMask;


    private void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        laserLine.SetPosition(0, laserOrigin.position);
        laserLine.enabled = false;

        RaycastHit hit;

        if (!Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, distance)) return;

        laserLine.enabled = true;
        laserLine.SetPosition(1, hit.point);
        Debug.Log("in");

    }
}

//else
//{
//    laserLine.SetPosition(1, laserOrigin.forward * distance);
//}