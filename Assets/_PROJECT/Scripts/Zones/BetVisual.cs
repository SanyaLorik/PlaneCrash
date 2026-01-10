using System;
using TMPro;
using UnityEngine;

public class BetVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _playerBank;
    [SerializeField] private TMP_Text _playerBetVisual;
    [SerializeField] private TMP_Text _xMultiplyVisual;
    [SerializeField] private TMP_Text _rewardVisual;
    [SerializeField] private TMP_Text _distanceVisual;
    [SerializeField] private PlayerStateManager _playerStateManager;
    // Пока банк тут
    [SerializeField] private PlayerBank _bank;
    
    

    private void Start() {
        ZoneManager.Instance.OnChooseBet += ShowBet;
        ZoneManager.Instance.OnChooseMultiplyer += ShowMultiplyer;
        _bank.OnBankChanged += ChangeBank;
        _playerStateManager.OnChangeState += OnChangeState;
    }

    private void OnChangeState(PlayerState state) {
        _playerBetVisual.text = "";
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";
    }


    private void OnDisable() {
        if (ZoneManager.Instance != null) {
            ZoneManager.Instance.OnChooseBet -= ShowBet;
            ZoneManager.Instance.OnChooseMultiplyer -= ShowMultiplyer;
        }
        _bank.OnBankChanged -= ChangeBank;
        _playerStateManager.OnChangeState -= OnChangeState;
    }

    private void ChangeBank(float capital) {
        _playerBank.text = $"Баланс: {capital:F2}";
    }

    private void ShowBet(float bet) {
        _playerBetVisual.text = $"Ставка: {bet:F2}";
        _xMultiplyVisual.text = "";
        _rewardVisual.text = "";
        _distanceVisual.text = "";

    }
    
    private void ShowMultiplyer(float multiplyer) {
        _xMultiplyVisual.text = $"Множитель: x{multiplyer}";
        _rewardVisual.text = $"Выигрышь: {ZoneManager.Instance.CurrentBet *  multiplyer:F2}";
        _distanceVisual.text = $"До финиша: {ZoneManager.Instance.CruiserDistance}м.";
    }

}
