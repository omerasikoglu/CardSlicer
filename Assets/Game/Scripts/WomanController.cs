using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class WomanController : Model
{

    [SerializeField] private List<Transform> collectiblePartList;
    [SerializeField] private List<ParticleSystem> particleList; //0=>good, 1=>bad, 2=>lose

    private void Awake()
    {
        DisableAllWomanParts();
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("in");
        Collectible coll = collision.GetComponentInParent<Collectible>();
        if (coll != null)
        {
            EnableWomanPart(coll.GetCollectionType());

            transform.DOScaleY(transform.localScale.y + 0.5f, 1f).SetEase(Ease.InBounce);
            Destroy(collision.gameObject);
        }
    }

    #region Anim
    private void DisableAllWomanParts()
    {
        foreach (Transform part in collectiblePartList)
        {
            part.gameObject.SetActive(false);
        }
    }

    public void EnableWomanPart(CollectibleType type)
    {
        collectiblePartList[(int)type].gameObject.SetActive(true);
    } 
    #endregion

    #region FX
    public void PlayGoodFX()
    {
        PlayFX(particleList[0]);
    }
    public void PlayBadFX()
    {
        PlayFX(particleList[1]);
    }
    public void PlayLoseFX()
    {
        PlayFX(particleList[2]);
    }
    private void PlayFX(ParticleSystem particle, int isOldEmitPending = 0)
    {
        //isOldEmitPending => 0 stop instantly
        foreach (ParticleSystem st in particleList)
        {
            st.Stop(true, (ParticleSystemStopBehavior)isOldEmitPending);
        }
        particle.Play();
    } 
    #endregion

}
