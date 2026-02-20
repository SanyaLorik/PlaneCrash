using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    
    [SerializeField] private TMP_Text PlayerCapitalVisual;
    [SerializeField] private MoneyCube _cube;
    
    
    public event Action<long> BankChanged;
    public event Action<int> MoneyCollect;

    public long PlayerCapital {
        get => _gameSave.GetSave.Money;
        private set => _gameSave.GetSave.Money = value;
    }

    public long PlayerRecord {
        get => _gameSave.GetSave.RecordMoney; 
        private set => _gameSave.GetSave.RecordMoney = value;
    }

    [Inject] IGameSave<GameSavePC> _gameSave;
    
    private void Start() {
        BankChanged += OnBankChanged;
        OnBankChanged(PlayerCapital);
    }

    private void OnBankChanged(long amount) {
        _gameSave.GetSave.Money = amount;
        _cube.SetMoneyAmount(PlayerCapital);
        _gameSave.Save();
    }


    public void AddMoney(double amount) {
        if (amount < 0) return;
        PlayerCapital += (long)amount;
        if (PlayerRecord < PlayerCapital) {
            PlayerRecord = PlayerCapital;
        }
        BankChanged?.Invoke(PlayerCapital);
    }
    
    
    public void AddFlightMoney(int amount) {
        if (amount < 0) return;
        PlayerCapital += amount;
        MoneyCollect?.Invoke(amount);
    }
    
    
    
    public void Buy(double amount) {
        if (amount > PlayerCapital) return;
        PlayerCapital -= (long)amount;
        BankChanged?.Invoke(PlayerCapital);
    }


    public bool CanBuy(double amount) =>
        PlayerCapital >= amount;
    
    public void GiveMeYourFuckingMoneyNigga(float amount) {
        if (amount > PlayerCapital) {
            Debug.LogWarning("Как ты сука поставил денег больше чем у тебя было");
            PlayerCapital = 0;
        }
        PlayerCapital -= (long)amount;
        BankChanged?.Invoke(PlayerCapital);
    }
    

}
