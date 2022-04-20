using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using DG.Tweening;
using TMPro;

public class PlayerController : Model {
    [SerializeField, Foldout("[Options]")] private float rotationSpeed = 3f, rotationLimitZ = .65f, eulerAngleLimitZ = 75f;
    [SerializeField, Foldout("[Options]")] private int unhappinessThreshold = 2;
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;

    [SerializeField] private Transform playerRoot;
    [SerializeField] private WomanController womanController;

    [SerializeField] private TextMeshProUGUI textMesh;

    [SerializeField, Foldout("[Card]")] private List<Material> materialList;
    private Material currentMaterial;
    [SerializeField, Foldout("[Card]")] private MeshRenderer cardMeshRenderer;
    [SerializeField, Foldout("[Card]")] private ParticleSystem upgrade1FX, upgrade2FX;

    private Vector3 womanFirstPos;
    private Vector3 handFirstPos;

    private BoxCollider cardCollider;
    private bool isTouchingScreen, canMove;

    private Vector3 goCoord, worldOffsetPos;
    private Vector3 eulerLeft, eulerRight;

    private void Awake() {
        GameManager.OnStateChanged += GameManager_OnStateChanged;
        InputObserver();
        SetEulerLimits();

        cardCollider = transform.GetComponentInChildren<BoxCollider>();

        handFirstPos = transform.position;
        womanFirstPos = womanController.transform.position;

        currentMaterial = materialList[0];
    }

    private void GameManager_OnStateChanged(GameState obj) {
        switch (obj) {
            case GameState.TapToPlay: ReloadPositions(); break;
            case GameState.Run: SetMovementSpeed(2); break;
            case GameState.Win: SetMovementSpeed(0); break;
            case GameState.Lose: SetMovementSpeed(0); break;
            default: break;
        }
    }

    protected override void Update() {
        base.Update();
        CheckRotationLimits();
    }
    private void OnTriggerEnter(Collider collision) {

        if (collision.CompareTag(StringData.EXIT)) {
            //Debug.Log("item exit");
            IncreaseUnhappiness();
        }

    }

    public Vector3 GetWomanPosition() {
        return womanController.transform.position;
    }
    public void ItemCollected(Collectible collectible) {

        textMesh.SetText($"{PlayerPrefs.GetInt(StringData.PREF_MONEY)}$");
        CheckCardMaterial();
    }

    public void CheckCardMaterial() {
        int currentMoney = PlayerPrefs.GetInt(StringData.PREF_MONEY);

        int moneyAmount = currentMoney / 300;
        cardMeshRenderer.material = moneyAmount switch
        {
            0 => materialList[0],
            1 => materialList[1],
            2 => materialList[2],
            _ => materialList[2],
        };

        //if (cardMeshRenderer.material == currentMaterial) return;

        switch (moneyAmount) {
            case 1: PlayFX(upgrade1FX); break;
            case 2: PlayFX(upgrade2FX); break;
            default: break;
        }
        currentMaterial = cardMeshRenderer.material;
    }
    private void PlayFX(ParticleSystem particle, int isOldEmitPending = 0) {
        //isOldEmitPending => 0 stop instantly
        upgrade1FX.Stop(true, (ParticleSystemStopBehavior)isOldEmitPending);
        upgrade2FX.Stop(true, (ParticleSystemStopBehavior)isOldEmitPending);

        particle.Play();
    }
    private void IncreaseUnhappiness() {

        if (PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS, 0) + 1 > unhappinessThreshold) return;

        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS,
            PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS, 0) + 1);

        if (PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS) < unhappinessThreshold) {
            //TODO: Woman starts to crying

        }
        else {
            GameManager.Instance.ChangeState(GameState.Lose);
        }
    }

    public void ReloadPositions() {

        SetMovementSpeed(0);
        transform.position = handFirstPos;
        womanController.transform.position = womanFirstPos;

        CheckCardMaterial();
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
        //Ray ray = UtilsClass.GetScreenPointToRay(coord);
        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        isTouchingScreen = true;
    }
    private void OnTouchEnded() {
        isTouchingScreen = false;
    }
    private void OnSlideMovementPerformed(Vector2 slideOffset) {
        if (!isTouchingScreen || !canMove) return;
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
