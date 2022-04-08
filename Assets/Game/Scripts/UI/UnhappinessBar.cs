using UnityEngine;
using Unity.Collections;
using NaughtyAttributes;
using System.Collections;

public class UnhappinessBar : Singleton<UnhappinessBar>
{
    [SerializeField, Foldout(StringData.OPTIONS)] private float lerpSpeed = 2f;
    [SerializeField, Foldout(StringData.OPTIONS)] private float maxProgress = 2f;
    [ShowNonSerializedField, Foldout(StringData.OPTIONS)] private float currentProgress;

    private Transform barTransform;

    private void Awake()
    {
        Init();
        ResetBar();
    }
    private void Start()
    {
        barTransform.localScale = new Vector3(0f, 1f, 1f);
    }
    private void Update()
    {
        CheckGreenBar();
    }
    private void Init()
    {
        barTransform = transform.Find(StringData.BAR);
    }
    private void CheckGreenBar()
    {
        barTransform.localScale = new Vector3(Mathf.Lerp(
            barTransform.localScale.x, UpdateProgressAmountNormalized(), lerpSpeed * Time.deltaTime), 1f, 1f);

        if (currentProgress < maxProgress) return;

        //? post mortem
    }
    [Button]
    public void IncreaseUnhappiness(int amount = 1)
    {
        if (currentProgress + amount > maxProgress) return;

        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS,
            PlayerPrefs.GetInt(StringData.PREF_UNHAPPINESS, 0) + 1);
        currentProgress += amount;
    }
    [Button]
    public void ResetBar()
    {
        currentProgress = 0;
    }
    public int GetScore()
    {
        return (int)currentProgress;
    }
    private float UpdateProgressAmountNormalized()
    {
        return (currentProgress / maxProgress);
    }
}