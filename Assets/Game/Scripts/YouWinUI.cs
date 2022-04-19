using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinUI : MonoBehaviour {
    [SerializeField] private PlayerController player;
    [SerializeField] private WomanController woman;
    private Vector3 womanFirstPos;
    private Vector3 handFirstPos;

    private void Awake() {
        handFirstPos = player.transform.position;
        womanFirstPos = woman.transform.position;
    }

    public void Reload() { //activate from ButtonUI
        player.transform.position = handFirstPos;
        woman.transform.position = womanFirstPos;

        PlayerPrefs.SetInt(StringData.PREF_MONEY, 0);
        PlayerPrefs.SetInt(StringData.PREF_UNHAPPINESS, 0);
        player.CheckCardMaterial();

        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        GameManager.Instance.ChangeState(GameState.Run);

    }
}
