using System;
using UnityEngine;
using Zenject;

public class StartZone : MonoBehaviour {
    
    [Inject] private BoostSpawner _boostSpawner;

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerStateManager stateManager)) {
            // Бусты уже готовы на этапе выбора множителя
            if (stateManager.CurrentState == PlayerState.Flight) return;
            stateManager.ChangePlayerState(PlayerState.Flight);
            _boostSpawner.SpawnEntranceBoost();
        }    
    }
}
