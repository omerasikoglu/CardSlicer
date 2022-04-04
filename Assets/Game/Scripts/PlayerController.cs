using System;
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isVerticalSwerveActive, isHorizontalSwerveActive;

    private Vector3 goCoord, worldOffsetPos;
    private bool isTouchingObject;
    public Camera cam;

    private void Awake()
    {
        Observer();
    }

    private void Observer()
    {
        inputManager.OnSwervePerformed += OnSwervePerformed;
        inputManager.OnTouchPerformed += OnTouchPerformed;
        inputManager.OnTouchEnded += OnTouchEnded;
    }

    private void OnTouchEnded()
    {
        isTouchingObject = false;
    }

    private void OnTouchPerformed(Vector2 coord)
    {
        Ray ray = UtilsClass.GetScreenPointToRay(coord);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, 150f)) return;
        if (hit.collider.CompareTag(" "))
            isTouchingObject = true;
    }


    private void OnSwervePerformed(Vector2 swerveOffset)
    {
        if (!isTouchingObject) return;
        //coord:ekranda, pos:dünyada, offsette z deðiþmez

        Vector3 goWorldPosition = gameObject.transform.position;

        goCoord = UtilsClass.GetWorldToScreenPoint(goWorldPosition); //Z'si 10 ekranda 
        worldOffsetPos = UtilsClass.GetScreenToWorldPoint(goCoord + (Vector3)swerveOffset);

        if (isVerticalSwerveActive) VerticalSwerve();
        if (isHorizontalSwerveActive) HorizontalSwerve();
    }

    private void VerticalSwerve()
    {
        transform.position = new Vector3(transform.position.x, worldOffsetPos.y, transform.position.z);
    }
    private void HorizontalSwerve()
    {
        transform.position = new Vector3(worldOffsetPos.x, transform.position.y, transform.position.z);
    }
}
