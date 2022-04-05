using System;
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.InputSystem;
// ReSharper disable All

public class PlayerController : MonoBehaviour
{
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;
    [SerializeField, Foldout("[Movement]")] private float movementSpeed = 8f, rotationSpeed = 3f;

    [SerializeField] private Transform playerRoot, womanRoot;
    [SerializeField] private BoxCollider cardCollider;

    private Vector3 goCoord, worldOffsetPos;
    private bool isTouchingScreen;
    private bool canMove = true;

    private void Awake()
    {
        InputObserver();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            Debug.Log("carded");
            Destroy(collision.GetComponentInParent<Transform>().gameObject);
        }
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
