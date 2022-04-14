using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        InitPlayerPrefs();
    }

    private static void InitPlayerPrefs()
    {
        PlayerPrefs.SetInt(StringData.PREF_MONEY, 900);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
    }
}
