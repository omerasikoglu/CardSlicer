using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[Serializable]
public enum Cam {
    PreRun = 0,
    Running = 1,
    FinalPose = 2,
}
public class CameraHandler : MonoBehaviour {

    [SerializeField] private List<CinemachineVirtualCamera> vCamList;

    private CinemachineVirtualCamera currentCam;
    private Cam? oldCam; //bcs while first time OpenCam() returns 0

    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;
    private void GameManager_OnStateChanged(GameState gameState) {

        switch (gameState) {
            case GameState.TapToPlay: OpenCam(Cam.PreRun); break;
            case GameState.Run: OpenCam(Cam.Running); break;
            case GameState.Win: OpenCam(Cam.FinalPose); break;
            case GameState.Lose: OpenCam(Cam.FinalPose); break;
            default: break;
        }
    }

    private void OpenCam(Cam newCam) {
        if (oldCam == newCam) return;

        oldCam = newCam;

        currentCam = vCamList[(int)newCam];

        ActivateNewCam(currentCam);
    }

    private void ActivateNewCam(CinemachineVirtualCamera newCam) {
        foreach (CinemachineVirtualCamera cam in vCamList) {
            cam.gameObject.SetActive(newCam == cam);
        }
    }
}
