using System;
using System.Collections;
using Architecture_M;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Zenject.Internal;

public class Trampoline : MonoBehaviour {
    [SerializeField] private float _jumpMultiplier = 1.5f; 
    [SerializeField] private float _firstJumpForce = 10f; 
    [SerializeField] private float _rewardForDistance = 10f; 
    [SerializeField] private TMP_Text _scoreText; 
    
    private float _jumpForceCurrent;
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private PlayerBank _bank;
    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] protected IGameSave<GameSavePC> _gameSave;
    

    private void Start() {
        _jumpForceCurrent = _firstJumpForce;
        _scoreText.text = _gameSave.GetSave.CountBatutJumps.ToString();
    }

    private void OnEnable() {
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Walking) {
            _jumpForceCurrent = _firstJumpForce;
        }

    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement player)) {
            _playerStateManager.ChangePlayerState(PlayerState.TrampolineJumping);
            _jumpForceCurrent *= _jumpMultiplier;
            player.AddVerticalImpulse(_jumpForceCurrent);
            Debug.Log("Прыжок! " + _jumpForceCurrent);
            StartCoroutine(JumpPlusBabki(player.Transform));
        }
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
        _gameSave.GetSave.CountBatutJumps++;
        _scoreText.text = _gameSave.GetSave.CountBatutJumps.ToString();
        _bank.AddMoney(GetMoneyReward(distance) * _upgradesCalculator.GetUpgradeMultiplierByLevel());

        // _money2dSpawner.SpawnOneMoneyNearPlayer();
        _money2dSpawner.SpawnOneMoneyInPoint(transform.position);
        
    }

    private float GetMoneyReward(float distance) {
        print("Награда за прыжок: " + distance * _rewardForDistance);
        return distance * 100f;
    }
}
