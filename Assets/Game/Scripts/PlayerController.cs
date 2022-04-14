using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class PlayerController : Model {
    [SerializeField, Foldout("[Options]")] private float rotationSpeed = 3f, rotationLimitZ = .65f, eulerAngleLimitZ = 75f;
    [SerializeField, Foldout("[Options]")] private int unhappinessThreshold = 2;
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;

    [SerializeField] private Transform playerRoot;
    [SerializeField] private WomanController womanController;

    private BoxCollider cardCollider;
    [SerializeField, SortingLayer] private LayerMask targetLayerMask;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Material emissionMaterial;

    //[SerializeField] private Animator animator;
    private bool isTouchingScreen, canMove = true;

    private Vector3 goCoord, worldOffsetPos;
    private Vector3 eulerLeft, eulerRight;

    private void Awake() {
        InputObserver();
        SetEulerLimits();

        cardCollider = transform.GetComponentInChildren<BoxCollider>();
    }
    protected override void Update() {
        base.Update();
        CheckRotationLimits();
    }

    private void FixedUpdate() {
        Lighto();
    }

    private void Lighto() {
        Bounds bounds = cardCollider.bounds;
        float rayLength = 2f;

        bool isHitCollectible = Physics.BoxCast(
            center: new Vector3(bounds.center.x, bounds.center.y, bounds.max.z),
            halfExtents: bounds.extents,
            direction: Vector3.forward,
            orientation: Quaternion.identity,
            maxDistance: rayLength,
            layerMask: targetLayerMask
            );

        Color rayColor = isHitCollectible ? Color.green : Color.red;
        emissionMaterial.SetColor("_EmissionColor", rayColor);
        DrawRectangleRay(cardCollider.bounds, Vector3.forward, rayLength, rayColor);

    }

    private void DrawRectangleRay(Bounds bounds, Vector3 dir, float rayLength, Color rayColor) {
        Debug.DrawRay( //top right
            start: bounds.max,
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //top left
            start: new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //bottom right
            start: new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //bottom left
            start: new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
    }
    private void OnTriggerEnter(Collider collision) {
        Collectible collectible = collision.GetComponent<Collectible>();

        if (collectible != null)
        {
            Debug.Log("collected");
            AffordMoney(collectible);
        }

        if (collision.CompareTag(StringData.EXIT))
        {
            Debug.Log("exit");
            IncreaseUnhappiness();
        }
    }
    private void AffordMoney(Collectible collectible) {
        int moneyAmount = collectible.GetItemDetails().money;
        int currentMoney = PlayerPrefs.GetInt(StringData.PREF_MONEY, 0);

        if (currentMoney + moneyAmount < 0)
        {
            Debug.Log("not enough money");
            womanController.PlayBadFX();
            return;
        }

        if (moneyAmount >= 0 && PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS) > 0) UnhappinessBar.Instance.ResetBar();

        currentMoney += moneyAmount;
        PlayerPrefs.SetInt(StringData.PREF_MONEY, currentMoney);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
        textMesh.SetText($"{PlayerPrefs.GetInt(StringData.PREF_MONEY)}$");

        womanController.PlayGoodFX();
        collectible.PlayCollectibleTasks();
    }
    private void IncreaseUnhappiness() {
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
    private void SetEulerLimits() {
        var localEulerAngles = playerRoot.localEulerAngles;

        eulerLeft = new Vector3(localEulerAngles.x, localEulerAngles.y, eulerAngleLimitZ);
        eulerRight = new Vector3(localEulerAngles.x, localEulerAngles.y, -eulerAngleLimitZ);
    }
    private void CheckRotationLimits() {
        if (playerRoot.rotation.z > rotationLimitZ) playerRoot.localEulerAngles = eulerLeft;
        if (playerRoot.rotation.z < -rotationLimitZ) playerRoot.localEulerAngles = eulerRight;
    }
    #endregion

    #region Input
    private void InputObserver() {
        inputManager.OnSlidePerformed += OnSlideRotatePerformed;
        inputManager.OnSlidePerformed += OnSlideMovementPerformed;
        inputManager.OnTouchPerformed += OnTouchPerformed;
        inputManager.OnTouchEnded += OnTouchEnded;
    }

    private void OnTouchPerformed(Vector2 coord) {
        Ray ray = UtilsClass.GetScreenPointToRay(coord);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        //if (!Physics.Raycast(ray, out RaycastHit hit, 150f)) return; if (hit.collider.CompareTag("Player"))  
        isTouchingScreen = true;
    }
    private void OnTouchEnded() {
        isTouchingScreen = false;
    }
    private void OnSlideMovementPerformed(Vector2 slideOffset) {
        if (!isTouchingScreen) return;
        if (!isSlideMovementYActive && !isSlideMovementXActive) return;

        Vector3 goWorldPosition = gameObject.transform.position;

        goCoord = UtilsClass.GetWorldToScreenPoint(goWorldPosition); //Z'si 10 ekranda 
        worldOffsetPos = UtilsClass.GetScreenToWorldPoint(goCoord + (Vector3)slideOffset);

        if (isSlideMovementYActive) SlideMovementY();
        if (isSlideMovementXActive) SlideMovementX();
    }
    private void OnSlideRotatePerformed(Vector2 slideOffset) {
        if (!isTouchingScreen) return;

        float rotatePosZ = -slideOffset.x * rotationSpeed * Time.deltaTime;

        if (isSlideRotateZActive) SlideRotateZ(rotatePosZ);
    }

    private void SlideRotateZ(float delta) {
        if (playerRoot.rotation.z > rotationLimitZ && delta > 0f) return;
        if (playerRoot.rotation.z < -rotationLimitZ && delta < 0f) return;

        //if (playerRoot.rotation.z + delta * 0.010 > 0.65f) return;
        //if (playerRoot.rotation.z + delta * 0.010 < -0.65f) return;

        playerRoot.Rotate(0, 0, delta);
    }
    private void SlideMovementY() {
        playerRoot.position = new Vector3(transform.position.x, worldOffsetPos.y, transform.position.z);
    }
    private void SlideMovementX() {
        playerRoot.position = new Vector3(worldOffsetPos.x, transform.position.y, transform.position.z);
    }
    #endregion
}
