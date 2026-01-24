using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class Boost : MonoBehaviour, IMagnetic {
    public AnimationCurve randomTrajectory;
    public Vector3 nextBooster;
    public bool hasTrap;

    
    private PlayerMovement _playerMovement;
    
    [Inject]
    public void Init(PlayerMovement playerMovement) {
        _playerMovement = playerMovement;
    }
    
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement player)) {
            if (player.IsBombed) {
                return;
            }
            Collect();
        }
        else if (collider.gameObject.TryGetComponent(out BotFlight bot)) {
            bot.SetBooster(randomTrajectory, nextBooster);
            Debug.Log("Бот налетел на буст");
        }
    }
    
    
    private void Collect() {
        Debug.Log("Начисление буста!");
        _playerMovement.SetBooster(randomTrajectory, nextBooster);
        gameObject.DisactiveSelf();
        CanBeMagnetic = false;
    }
    

    public Vector3 Position =>  transform.position;
    public MagneticType Type { get; } = MagneticType.Boost;


    public bool CanBeMagnetic { get; set; } = true;

    
    
    public void Attract(Vector3 target, float speed) {
        // Debug.Log("Притяжение буста " + transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            

        // Debug.Log(Vector3.SqrMagnitude(transform.position - target));
        if (Vector3.SqrMagnitude(transform.position - target) <= 1f) {
            Collect();
        }
    }
    
    
}
