using UnityEngine;

public class ChillZone : MonoBehaviour {
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            Debug.Log("ChangeMultiplyer ChillZone");
            ZoneManager.Instance.ChangeMultiplyer(0);
            ZoneManager.Instance.ChangeBet(0);
        }    
    }
    
    
}
