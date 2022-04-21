using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndArea : MonoBehaviour {
    [SerializeField] private Scoreboard scoreboard;

    private void OnTriggerEnter(Collider collision) {

        PlayerController player = collision.attachedRigidbody.GetComponent<PlayerController>();
        if (player != null) {
            GameManager.Instance.ChangeState(GameState.Scoreboard);
            player.SetCardScoreboardRiseHeight(scoreboard.GetScoreboardHeight());
        }
    }
}
