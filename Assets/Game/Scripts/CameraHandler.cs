using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEditor.Build.Reporting;
using UnityEngine;


[Serializable]
public enum Cam {
    PreRun = 0,
    Running = 1,
    FinalPose = 2,
    FinalPose2 = 3,
}
public class CameraHandler : MonoBehaviour {

    [SerializeField] private List<CinemachineVirtualCamera> vCamList;

    private CinemachineVirtualCamera currentCam;
    private Cam? oldCam; //bcs while first time OpenCam() returns 0
    private TimerClass timer;
    private void OnEnable() => GameManager.OnStateChanged += GameManager_OnStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= GameManager_OnStateChanged;
    private void GameManager_OnStateChanged(GameState gameState) {

        switch (gameState) {
            case GameState.TapToPlay: OpenCam(Cam.PreRun); break;
            case GameState.Run: OpenCam(Cam.Running); break;
            case GameState.Win:
                OpenCam(Cam.FinalPose);
                timer ??= new TimerClass
                {
                    timer = 3f
                };
                //StartCoroutine(UtilsClass.Wait(() =>
                //{
                //    OpenCam(Cam.FinalPose2);
                //}, 0f)); 
                break;
            case GameState.Lose: OpenCam(Cam.FinalPose); break;
            default: break;
        }
    }

    private void Update() {
        if (timer != null) timer.timer -= Time.deltaTime;
        if (timer?.timer <= 0f) OpenCam(Cam.FinalPose2);
    }

    private class TimerClass {
        public float timer;
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

namespace Test {
    class LINQTest : MonoBehaviour {
        enum BoolNames {
            asd = 0,
        }

        private BoolNames boolNames;
        private Dictionary<BoolNames, bool> newDic;

        private void Awake() {

            newDic = new Dictionary<BoolNames, bool>();
        }

        void St() {

            int totalTrue = newDic.Where
                ((t, i) => newDic[(BoolNames)i].Equals(true)).Count();
        }
    }
}
