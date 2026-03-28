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
    
    [Inject] TasksManager _tasksManager;
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _zoneManager.ChangeMultiplier(0);
            _playerStateManager.ChangePlayerState(PlayerState.Walking);
            _tasksManager.CheckToNeedLine();
        }    
    }
    
    
}
