using System;
using UnityEngine;

public class StartZone : MonoBehaviour {
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerStateManager stateManager)) {
            stateManager.ChangePlayerState(PlayerState.Flight);
        }    
    }
}
