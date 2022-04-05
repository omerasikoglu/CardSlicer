using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using DG.Tweening;

[SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;
    [SerializeField, Foldout("[Movement]")] private float movementSpeed = 8f, rotationSpeed = 3f;

    [SerializeField] private Transform playerRoot, womanRoot;

    private bool isTouchingScreen, canMove = true;

    private Vector3 goCoord, worldOffsetPos;
    private Vector3 eulerLeft, eulerRight;

    private void Awake()
    {
        InputObserver();
        InitEulerLimits();
    }

    private void InitEulerLimits()
    {
        eulerLeft = new Vector3(
            playerRoot.localEulerAngles.x, playerRoot.localEulerAngles.y, 75f
        );
        eulerRight = new Vector3(
            playerRoot.localEulerAngles.x, playerRoot.localEulerAngles.y, -75f
        );
    }

    private void Update()
    {
        HandleMovement();
        CheckRotationLimits();
    }

    private void CheckRotationLimits()
    {
        if (playerRoot.rotation.z > 0.65f) playerRoot.localEulerAngles = eulerLeft;
        if (playerRoot.rotation.z < -0.65f) playerRoot.localEulerAngles = eulerRight;
    }

    private void HandleMovement()
    {
        transform.position += movementSpeed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Collectible collectible = collision.GetComponentInParent<Collectible>();

        if (collectible == null) return;

        //Destroy(collision.GetComponentInParent<Transform>().gameObject);
    }

    #region Input
    private void InputObserver()
    {
        inputManager.OnSlidePerformed += OnSlideRotatePerformed;
        inputManager.OnSlidePerformed += OnSlideMovementPerformed;
        inputManager.OnTouchPerformed += OnTouchPerformed;
        inputManager.OnTouchEnded += OnTouchEnded;
    }

    private void OnTouchPerformed(Vector2 coord)
    {
        Ray ray = UtilsClass.GetScreenPointToRay(coord);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        //if (!Physics.Raycast(ray, out RaycastHit hit, 150f)) return; if (hit.collider.CompareTag("Player"))  
        isTouchingScreen = true;
    }
    private void OnTouchEnded()
    {
        isTouchingScreen = false;
    }
    private void OnSlideMovementPerformed(Vector2 slideOffset)
    {
        if (!isTouchingScreen) return;
        if (!isSlideMovementYActive && !isSlideMovementXActive) return;

        Vector3 goWorldPosition = gameObject.transform.position;

        goCoord = UtilsClass.GetWorldToScreenPoint(goWorldPosition); //Z'si 10 ekranda 
        worldOffsetPos = UtilsClass.GetScreenToWorldPoint(goCoord + (Vector3)slideOffset);

        if (isSlideMovementYActive) SlideMovementY();
        if (isSlideMovementXActive) SlideMovementX();
    }
    private void OnSlideRotatePerformed(Vector2 slideOffset)
    {
        if (!isTouchingScreen) return;

        float rotatePosZ = -slideOffset.x * rotationSpeed * Time.deltaTime;

        if (isSlideRotateZActive) SlideRotateZ(rotatePosZ);
    }

    private void SlideRotateZ(float delta)
    {
        if (playerRoot.rotation.z > 0.6f && delta > 0f) return;
        if (playerRoot.rotation.z < -0.6f && delta < 0f) return;

        //if (playerRoot.rotation.z + delta * 0.010 > 0.65f) return;
        //if (playerRoot.rotation.z + delta * 0.010 < -0.65f) return;

        playerRoot.Rotate(0, 0, delta);
    }
    private void SlideMovementY()
    {
        playerRoot.position = new Vector3(transform.position.x, worldOffsetPos.y, transform.position.z);
    }
    private void SlideMovementX()
    {
        playerRoot.position = new Vector3(worldOffsetPos.x, transform.position.y, transform.position.z);
    }
    #endregion
}
