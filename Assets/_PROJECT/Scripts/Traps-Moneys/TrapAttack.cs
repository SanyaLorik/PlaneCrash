using System;
using UnityEngine;
using Zenject;

public abstract class TrapAttack : MonoBehaviour {
    [SerializeField] private TrapVisual Visual;
    private BoostSpawner _boostSpawner;

    [Inject] 
    public void Init(BoostSpawner boostSpawner) {
        _boostSpawner = boostSpawner;
    }




    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement movement)) {
            PlayerGetTrapped(movement);
        }
        // Если бот еще одна логика
        if (collider.TryGetComponent(out BotStateManager bot)) {
            BotGetTrapped(bot);
        }
    }

    private void BotGetTrapped(BotStateManager bot) {
        Atack(bot.Rb);
        bot.PlayerInSpawn();
    }


    private void PlayerGetTrapped(PlayerMovement movement) {
       if(!movement.ObjectGetAllow) return;
       
       if (movement.TryToKill()) {
           Debug.Log("Killed");
           movement.SetPlayerIsBombed();
           Atack(movement.Rb);    
       }
       else {
           Debug.Log("Minus 1 shield!");
           Debug.Log("_boostSpawner = " + _boostSpawner);
           _boostSpawner.SetPlayerToNextRightBooster(movement.transform.position);
       }
       Visual.GetEffect();
   }

   protected abstract void Atack(Rigidbody rb);


}
