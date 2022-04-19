using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : Model {
    [SerializeField, Foldout("[Options]")] private float rotationSpeed = 3f, rotationLimitZ = .65f, eulerAngleLimitZ = 75f;
    [SerializeField, Foldout("[Options]")] private int unhappinessThreshold = 2;
    [SerializeField, Foldout("[Input]")] private InputManager inputManager;
    [SerializeField, Foldout("[Input]")] private bool isSlideMovementYActive, isSlideMovementXActive, isSlideRotateZActive;
    [SerializeField, Foldout("[Ray]")] private LayerMask targetLayerMask;
    [SerializeField, Foldout("[Ray]")] private Material emissionMaterial;

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
    private bool isTouchingScreen;

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
            case GameState.TapToPlay: SetMovementSpeed(0); break;
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

    private void FixedUpdate() {
        //Lighto();
    }

    private void Lighto() {
        Bounds bounds = cardCollider.bounds;
        float rayLength = 2f;
        RaycastHit hitInfo;

        bool isHitCollectible = Physics.BoxCast(
            center: new Vector3(bounds.center.x, bounds.center.y, bounds.max.z),
            halfExtents: new Vector3(bounds.extents.x, bounds.extents.y),
            direction: Vector3.forward,
            hitInfo: out hitInfo,
            orientation: Quaternion.identity,
            maxDistance: rayLength,
            layerMask: targetLayerMask
            );

        Color rayColor = isHitCollectible ? Color.green : Color.red;
        //Color rayColor = raycastHits.Length != 0 ? Color.green : Color.red;
        //emissionMaterial.SetColor("_EmissionColor", rayColor);
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
        if (collectible != null) {
            Debug.Log("collected");
            AffordMoney(collectible);
        }

        EndGameArea endGameArea = collision.GetComponentInParent<EndGameArea>();
        if (endGameArea != null) {
            Debug.Log("endgame");
            GameManager.Instance.ChangeState(GameState.Win);
        }

        if (collision.CompareTag(StringData.EXIT)) {
            Debug.Log("item exit");
            IncreaseUnhappiness();
            //Destroy(collision.GetComponentInParent<Rigidbody>().gameObject);
        }

    }
    private void AffordMoney(Collectible collectible) {
        int moneyAmount = collectible.GetItemDetails().money;
        int currentMoney = PlayerPrefs.GetInt(StringData.PREF_MONEY, 0);

        if (currentMoney + moneyAmount < 0) {
            Debug.Log("not enough money");
            //womanController.PlayBadFX();
            return;
        }

        //if (moneyAmount >= 0 && PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS) > 0) UnhappinessBar.Instance.ResetBar();

        currentMoney += moneyAmount;
        PlayerPrefs.SetInt(StringData.PREF_MONEY, currentMoney);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
        textMesh.SetText($"{PlayerPrefs.GetInt(StringData.PREF_MONEY)}$");

        CheckCardMaterial();
        collectible.PlayHealUpFX();
        womanController.PlayGoodFX();
        collectible.PlayCollectibleTasks();
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
            StartCoroutine(womanController.PlayTearsWhileWalking());
        }
        else {
            GameManager.Instance.ChangeState(GameState.Lose);
        }
    }

    public void Reload() { //activate from UI UnityEvent
        transform.position = handFirstPos;
        womanController.transform.position = womanFirstPos;

        PlayerPrefs.SetInt(StringData.PREF_MONEY, 0);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
        CheckCardMaterial();

        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        GameManager.Instance.ChangeState(GameState.Run);

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
