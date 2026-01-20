using System;
using UnityEngine;

public class Boost : MonoBehaviour {
    public AnimationCurve randomTrajectory;
    public Vector3 nextBooster;

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement player)) {
            player.SetBooster(randomTrajectory, nextBooster);
            gameObject.SetActive(false);
        }
        else if (collider.gameObject.TryGetComponent(out BotFlight bot)) {
            bot.SetBooster(randomTrajectory, nextBooster);
            Debug.Log("Бот налетел на буст");
        }
    }
}
