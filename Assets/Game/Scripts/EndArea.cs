using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndArea : MonoBehaviour {
    [SerializeField] private Scoreboard scoreboard;

    private void OnTriggerEnter(Collider collision) {

        if (collision.CompareTag(StringData.CARD))
        {
            GameManager.Instance.ChangeState(GameState.Win);
        }
    }
}
