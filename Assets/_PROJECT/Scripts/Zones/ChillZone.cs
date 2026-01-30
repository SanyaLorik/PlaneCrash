using UnityEngine;
using Zenject;

public class ChillZone : MonoBehaviour {
    
    private ZoneManager _zoneManager;
    private PlayerStateManager _playerStateManager;
        
    [Inject]
    public void Init(ZoneManager zoneManager, PlayerStateManager playerStateManager) {
        _zoneManager = zoneManager;
        _playerStateManager = playerStateManager;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _zoneManager.ChangeMultiplyer(0);
            _zoneManager.ChangeBet(0);
            _playerStateManager.ChangePlayerState(PlayerState.Walking);
        }    
    }
    
    
}
