using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateChanger : MonoBehaviour {
   [SerializeField] private PlayerState _playerState;

   private void OnTriggerEnter(Collider collider) {
       if(collider.TryGetComponent(out PlayerStateManager playerStateManager)) { 
           playerStateManager.ChangePlayerState(_playerState);
       }
   }
}
