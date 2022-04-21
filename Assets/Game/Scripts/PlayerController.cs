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
    [SerializeField, Foldout("[Input]")] private bool isSlideRotateZActive;

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
    [SerializeField, ReadOnly] private bool isTouchingScreen, canInput;

    private Vector3 goCoord, worldOffsetPos;
    private Vector3 eulerLeft, eulerRight;

    private void Awake()
    {

        SetEulerLimits();

        cardCollider = transform.GetComponentInChildren<BoxCollider>();

        handFirstPos = transform.position;
        womanFirstPos = womanController.transform.position;

        currentMaterial = materialList[0];
    }

    private void OnEnable() {
        GameManager.OnStateChanged += GameManager_OnStateChanged;
        inputManager.OnSlidePerformed += OnSlideRotatePerformed;
        inputManager.OnSlidePerformed += OnSlideMovementPerformed;
        inputManager.OnTouchPerformed += OnTouchPerformed;
        inputManager.OnTouchEnded += OnTouchEnded;
    }
    private void OnDisable()
    {
        GameManager.OnStateChanged -= GameManager_OnStateChanged;
        DisableInputs();
    }

    private void DisableInputs()
    {
        inputManager.OnSlidePerformed -= OnSlideRotatePerformed;
        inputManager.OnSlidePerformed -= OnSlideMovementPerformed;
        inputManager.OnTouchPerformed -= OnTouchPerformed;
        inputManager.OnTouchEnded -= OnTouchEnded;
    }


    private void GameManager_OnStateChanged(GameState obj) {
        switch (obj) {
            case GameState.TapToPlay:
                ReloadPositions();
                canInput = false; break;
            case GameState.Run: SetMovementSpeed(2); canInput = true; break;

            case GameState.Win: SetMovementSpeed(0); canInput = false; break;
            case GameState.Lose: SetMovementSpeed(0); canInput = false; break;
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
    private void OnTouchPerformed(Vector2 coord) {
        if (!canInput) return;
        isTouchingScreen = true;
    }
    private void OnTouchEnded() {
        if (!canInput) return;
        isTouchingScreen = false;
    }
    private void OnSlideMovementPerformed(Vector2 slideOffset) {
        if (!isTouchingScreen) return;
        if (!canInput) return;

        Vector3 goWorldPosition = gameObject.transform.position;

        goCoord = UtilsClass.GetWorldToScreenPoint(goWorldPosition); //Z'si 10 ekranda 
        worldOffsetPos = UtilsClass.GetScreenToWorldPoint(goCoord + (Vector3)slideOffset);
    }
    private void OnSlideRotatePerformed(Vector2 slideOffset) {

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
    #endregion
}
