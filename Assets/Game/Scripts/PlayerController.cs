using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using DG.Tweening;

public class PlayerController : Model
{
    [SerializeField, Foldout("[Options]")] private float rotationSpeed = 3f, rotationLimitZ = .65f, eulerAngleLimitZ = 75f;
    [SerializeField, Foldout("[Options]")] private int unhappinessThreshold = 2;
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;
   
    [SerializeField] private Transform playerRoot;
    [SerializeField] private WomanController womanController;
    [SerializeField] private BoxCollider cardCollider;

    //[SerializeField] private Animator animator;

    private bool isTouchingScreen, canMove = true;
    private int currentMoney;

    private Vector3 goCoord, worldOffsetPos;
    private Vector3 eulerLeft, eulerRight;

    private void Awake()
    {
        InputObserver();
        SetEulerLimits();

        currentMoney = PlayerPrefs.GetInt(StringData.PREF_MONEY, 0);
    }
    protected override void Update()
    {
        base.Update();
        CheckRotationLimits();
        DrawRay();
    }

    private void DrawRay()
    {
       
    }

    private void OnTriggerEnter(Collider collision)
    {
        Collectible collectible = collision.GetComponentInParent<Collectible>();

        if (collectible != null)
        {
            AffordMoney(collectible);
        }

        Exit exit = collision.GetComponentInParent<Exit>();

        if (exit != null)
        {
            IncreaseUnhappiness();
        }
    }
    private void AffordMoney(Collectible collectible)
    {
        int moneyAmount = collectible.GetMoneyAmountOfCollectible();

        if (currentMoney + moneyAmount < 0)
        {
            //Debug.Log("yeterli paran yok");
            womanController.PlayBadFX();
            return;
        }

        if (moneyAmount >= 0 && PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS) > 0) UnhappinessBar.Instance.ResetBar();

        currentMoney += moneyAmount;
        PlayerPrefs.SetInt(StringData.PREF_MONEY, currentMoney);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);

        womanController.PlayGoodFX();
        collectible.PlayCollectibleTasks();
    }
    private void IncreaseUnhappiness()
    {
        UnhappinessBar.Instance.IncreaseUnhappiness();

        if (PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS) < unhappinessThreshold)
        {
            womanController.PlayBadFX();
        }
        else
        {
            womanController.PlayLoseFX();
        }
    }

    #region Rotation
    private void SetEulerLimits()
    {
        var localEulerAngles = playerRoot.localEulerAngles;

        eulerLeft = new Vector3(localEulerAngles.x, localEulerAngles.y, eulerAngleLimitZ);
        eulerRight = new Vector3(localEulerAngles.x, localEulerAngles.y, -eulerAngleLimitZ);
    }
    private void CheckRotationLimits()
    {
        if (playerRoot.rotation.z > rotationLimitZ) playerRoot.localEulerAngles = eulerLeft;
        if (playerRoot.rotation.z < -rotationLimitZ) playerRoot.localEulerAngles = eulerRight;
    }
    #endregion

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
        if (playerRoot.rotation.z > rotationLimitZ && delta > 0f) return;
        if (playerRoot.rotation.z < -rotationLimitZ && delta < 0f) return;

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
