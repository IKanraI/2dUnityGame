using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winCondition : MonoBehaviour {
    PlayerActionControls playerControls;

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {
            playerControls.Land.Interact.performed += _ => {
                GetComponent<Animator>().SetTrigger("Open");
            };       
        }
    }

    private void Awake() {
        playerControls = new PlayerActionControls();
    }

    private void OnEnable() {
        playerControls.Enable();
    }
    
    private void OnDisable() {
        playerControls.Disable();
    }
}
