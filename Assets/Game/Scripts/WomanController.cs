using System;
using System.Collections;
using System.Collections.Generic;
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
