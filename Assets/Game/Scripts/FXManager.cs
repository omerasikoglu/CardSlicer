using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : Singleton<FXManager>
{
    public void PlayFX(ParticleSystem fx, Transform tramsform)
    {
        fx.transform.position = tramsform.position;
        fx.Play();
    }
}
