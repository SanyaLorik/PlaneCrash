using System;
using UnityEngine;
using Zenject;

public class StartZone : MonoBehaviour {
    
    private BoostSpawner _boostSpawner;
    
    
    [Inject]
    public void Init(BoostSpawner boostSpawner) {
        _boostSpawner = boostSpawner;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerStateManager stateManager)) {
            _boostSpawner.SpawnEntranceBoost();
            stateManager.ChangePlayerState(PlayerState.Flight);
            
        }    
    }
}
