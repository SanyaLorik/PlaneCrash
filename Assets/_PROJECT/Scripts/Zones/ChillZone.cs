using UnityEngine;
using Zenject;

public class ChillZone : MonoBehaviour {
    
    private ZoneManager _zoneManager;
        
    [Inject]
    public void Init(ZoneManager zoneManager) {
        _zoneManager = zoneManager;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            _zoneManager.ChangeMultiplyer(0);
            _zoneManager.ChangeBet(0);
        }    
    }
    
    
}
