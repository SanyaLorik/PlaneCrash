using System;
using SanyaBeerExtension;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class BetVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _playerBankText;
    [SerializeField] private TMP_Text _playerBetText;
    [SerializeField] private TMP_Text _rewardText;
    
    
    [SerializeField] private GameObject _betAndRewardCanvas;
    [SerializeField] private GameObject _mainCanvas;
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private ZoneManager _zoneManager;
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private PetsManager _petsManager; 
    [Inject] private PlayerBank _bank;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private NumberFormatter _formatter; 
    [Inject] private LocalizationDataPC _localization;


    private void OnEnable() {
        _bank.BankChanged += OnChangeBank;
        _zoneManager.ChooseBet += ShowBet;
        _zoneManager.ChooseMultiplier += ShowMultiplier;
        _playerStateManager.ChangeState += OnChangeState;
        _playerStats.ChangeStats += PlayerStatsOnChangeStats;
        _petsManager.BuyPet += PetsManagerOnBuyPet;
    }

    private void Start() {
        _betAndRewardCanvas.DisactiveSelf();
        _playerBankText.text = _formatter.ValuteFormatter(_bank.PlayerCapital);
    }
  



    private void PetsManagerOnBuyPet() {
        Debug.Log("Новые питомцы!");
        PlayerStatsOnChangeStats();
    }

    private void PlayerStatsOnChangeStats() {
        // Обновление вывода текста множителя где-то было 
        // text = string.Format(
        //     _localization.UpgradeMultiplierTemplate,
        //     $"{_upgradesCalculator.GetUpgradeMultiplierByLevel(true, true):F2}"
        // );
    }

 

    private void OnChangeState(PlayerState state) {
        _betAndRewardCanvas.DisactiveSelf();
        if (state == PlayerState.Flight) {
            _mainCanvas.DisactiveSelf();
        }
        else if(!_mainCanvas.activeSelf) {
            _mainCanvas.ActiveSelf();
        }
    }


    private void OnChangeBank(long capital) {
        _playerBankText.text = _formatter.ValuteFormatter(capital);
    }

    private void ShowBet(float bet) {
        _playerBetText.text = _formatter.ValuteFormatter(bet);
        _rewardText.text = _formatter.ValuteFormatter(_zoneManager.BetAmount);
        if (bet == 0 && _betAndRewardCanvas.activeSelf) {
            _betAndRewardCanvas.DisactiveSelf();
        }
        else if (bet > 0 && !_betAndRewardCanvas.activeSelf) {
            _betAndRewardCanvas.ActiveSelf();
        }
    }
    
    private void ShowMultiplier(float multiplier) {
        _rewardText.text = _formatter.ValuteFormatter(_zoneManager.BetAmount * multiplier);
    }

    
    
    private void OnDisable() {
        _bank.BankChanged -= OnChangeBank;
        _zoneManager.ChooseBet -= ShowBet;
        _zoneManager.ChooseMultiplier -= ShowMultiplier;
        _playerStateManager.ChangeState -= OnChangeState;
        _playerStats.ChangeStats -= PlayerStatsOnChangeStats;
        _petsManager.BuyPet -= PetsManagerOnBuyPet;
    }
    
}
