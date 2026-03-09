using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    [SerializeField] private MoneyCube _cube;
    
    
    public event Action<long> BankChanged;
    public event Action<long> BankNewMoneyPlus;
    public event Action<long> BankNewMoneyMinus;
    public event Action<long> MoneyCollect;

    [Inject] IGameSave<GameSavePC> _gameSave;
    [Inject] TutorialCompiller _tutorialCompiller;
    
    public long PlayerCapital {
        get => _gameSave.GetSave.Money;
        private set => _gameSave.GetSave.Money = value;
    }
    public long PlayerRecord {
        get => _gameSave.GetSave.RecordMoney; 
        private set => _gameSave.GetSave.RecordMoney = value;
    }

    private void ChangeMoney(long newMoney, bool save = true) {
        PlayerCapital += newMoney;
        _cube.SetMoneyAmountForBank(PlayerCapital);

        if (PlayerCapital < 0)
            PlayerCapital = 0;
        if (PlayerRecord < PlayerCapital)
            PlayerRecord = PlayerCapital;
        if (save)
            _gameSave.Save();
        
        
        BankChanged?.Invoke(PlayerCapital);

        if (newMoney > 0) {
            BankNewMoneyPlus?.Invoke(newMoney);
        }
        else {
            BankNewMoneyMinus?.Invoke(Math.Abs(newMoney));
        }
    }

    
    public void Buy(double amount) {
        if (!CanBuy(amount)) return;
        ChangeMoney((long)-amount);
    }
    
    
    public void GetSilentBetFallMoney(double amount) {
        if(!_tutorialCompiller.TutorialPassed) return;
        PlayerCapital -= (long)amount;
        BankChanged?.Invoke(PlayerCapital);
    }


    public void AddMoney(double amount) {
        if (amount <= 0) return;
        ChangeMoney((long)amount);
    }
    
    
    public void AddFlightMoney(long amount) {
        if (amount <= 0) return;
        MoneyCollect?.Invoke(amount);
        ChangeMoney(amount, false);
    }


    public bool CanBuy(double amount) =>
        PlayerCapital >= amount;


}
