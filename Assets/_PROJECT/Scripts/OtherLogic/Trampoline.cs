using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Zenject.Internal;

public class Trampoline : MonoBehaviour {
    [SerializeField] private float _jumpMultiplier = 1.5f; 
    [SerializeField] private float _firstJumpForce = 10f; 
    [SerializeField] private float _rewardForDistance = 10f; 
    
    private PlayerStateManager _playerStateManager;
    private PlayerBank _bank;
    private float _jumpForceCurrent;

    [Inject]
    private void Init(PlayerStateManager playerStateManager, PlayerBank bank) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;

        _bank = bank;
    }

    private void Awake() {
        _jumpForceCurrent = _firstJumpForce;
    }


    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Walking) {
            _jumpForceCurrent = _firstJumpForce;
        }

    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement player)) {
            Debug.Log("Прыжок!");
            _playerStateManager.ChangePlayerState(PlayerState.TrampolineJumping);
            player.Rb.linearVelocity = Vector3.zero;
            SetJump(player.Rb);
            StartCoroutine(JumpPlusBabki(player.Transform));
        }
    }

    private void SetJump(Rigidbody rb) {
        rb.AddForce(Vector3.up * _jumpForceCurrent, ForceMode.Impulse);
        _jumpForceCurrent *= _jumpMultiplier;
        Debug.Log(_jumpForceCurrent);
    }

    private IEnumerator JumpPlusBabki(Transform player) {
        float startY = player.position.y;
        float previousY = player.position.y;
        while (player.position.y >= previousY) {
            previousY = player.position.y;
            yield return null;
        }
        float distance =  player.position.y - startY;
        print($"Старт координата: {startY}, конечная: {player.position.y}, дистанция: {distance}" );
        _bank.AddMoney(GetMoneyReward(distance));
    }

    private float GetMoneyReward(float distance) {
        print("Награда за прыжок: " + distance * _rewardForDistance);
        return distance * 100f;
    }
}
