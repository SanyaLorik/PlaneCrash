using System;
using UnityEngine;

public class Boost : MonoBehaviour {
    public AnimationCurve randomTrajectory;
    public Vector3 nextBooster;

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.SetBooster(randomTrajectory, nextBooster);
            gameObject.SetActive(false);
        }
    }
}
