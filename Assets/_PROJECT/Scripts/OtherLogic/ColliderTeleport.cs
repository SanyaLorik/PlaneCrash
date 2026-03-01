using UnityEngine;

public class ColliderTeleport : MonoBehaviour {
     private void OnTriggerEnter(Collider collider){
        if (collider.TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.TpPlayerInParkour();
        }
     }
    
}
