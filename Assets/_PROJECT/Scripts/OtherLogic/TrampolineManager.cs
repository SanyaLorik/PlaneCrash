using System;
using System.Collections;
using Architecture_M;
using TMPro;
using UnityEngine;
using Zenject;

public class TrampolineManager : MonoBehaviour {
    [SerializeField] private float _jumpMultiplier = 1.01f; 
    [SerializeField] private float _firstJumpForce = 15f; 
    [SerializeField] private float _rewardForDistance = 25f; 
    [SerializeField] private TMP_Text _scoreText; 
    [SerializeField] private float _maxTrampolineDistance = 69; 
    [SerializeField, Range(0,1)] private double _trampolineRangPercentage = 0.000001f;
    
    private float _jumpForceCurrent;
    
    public event Action<Trampoline> OnTrampolineJump;
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private PlayerBank _bank;
    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] protected IGameSave<GameSavePC> _gameSave;
    [Inject] private RangManager _rangManager;
    [Inject] private NumberFormatter _formatter;
    [Inject] private TrampolineManager _trampolineManager;
    [Inject] private PlayerMovement _playerMovement;

    public void TrampolineJump(Trampoline trampoline) {
        _trampolineManager.OnTrampolineJump?.Invoke(trampoline);
        _playerStateManager.ChangePlayerState(PlayerState.TrampolineJumping);
        _jumpForceCurrent *= _jumpMultiplier;
        _jumpForceCurrent = Math.Clamp(_maxTrampolineDistance, 0f, _jumpForceCurrent);
        _playerMovement.AddVerticalImpulse(_jumpForceCurrent);
        Debug.Log("Прыжок! " + _jumpForceCurrent);
        StartCoroutine(JumpPlusBabki(_playerMovement.Transform));
    }
    
    
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
        
        double baseReward =
            _rangManager.GetNextRangePercentage(_trampolineRangPercentage);

        double skillReward =
            GetRewardForDistance(distance);

        double reward =
            (baseReward + skillReward) 
            *
            _upgradesCalculator.GetUpgradeMultiplierByLevel();

        _bank.AddMoney(reward);
        
        
        // _money2dSpawner.SpawnOneMoneyNearPlayer();
        _money2dSpawner.SpawnOneMoneyInPoint(player.transform.position);
        
    }

    private double GetRewardForDistance(float distance) {
        print("Награда за прыжок: " + distance * _rewardForDistance);
        return distance * _rewardForDistance;
    
    }
}