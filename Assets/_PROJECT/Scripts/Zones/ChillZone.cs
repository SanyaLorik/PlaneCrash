using UnityEngine;

public class ChillZone : MonoBehaviour {
    [SerializeField] private BetAccumulation _accumulationZone;

    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerBank bank)) {
            _accumulationZone.ResetBet();
        }    
    }
    
    
}
