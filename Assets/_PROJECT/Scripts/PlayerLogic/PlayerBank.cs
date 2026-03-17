using System;
using Architecture_M;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    [SerializeField] private MoneyCube _cube;
    [SerializeField] private long _maxTutorialAmount;
    
    
    public event Action<long> BankChanged;
    public event Action<long> BankNewMoneyPlus;
    public event Action<long> BankNewMoneyMinus;
    public event Action<long> MoneyCollect;

    [Inject] IGameSave<GameSavePC> _gameSave;
    [Inject] TutorialCompiller _tutorialCompiller;
    [Inject] PlayerConfig _playerConfig;


    private void Start() {
        if (PlayerCapital < _playerConfig.StartMoneyAmount) {
            AddMoney(_playerConfig.StartMoneyAmount);
        }
    }


    public long PlayerCapital {
        get => _gameSave.GetSave.Money;
        private set => _gameSave.GetSave.Money = value;
    }
    public long PlayerRecord {
        get => _gameSave.GetSave.RecordMoney; 
        private set => _gameSave.GetSave.RecordMoney = value;
    }

    private void ChangeMoney(long newMoney, bool save = true) {
        if (newMoney > 0) {
            TryAddMoney(newMoney);
        }
        else {
            PlayerCapital += newMoney;
            BankNewMoneyMinus?.Invoke(Math.Abs(newMoney));
            if (PlayerCapital < 0)
                PlayerCapital = 0;
        }


        if (PlayerRecord < PlayerCapital) {
            PlayerRecord = PlayerCapital;
        }
        if (save) {
            _gameSave.Save();
        }
        _cube.SetMoneyAmountForBank(PlayerCapital);
        BankChanged?.Invoke(PlayerCapital);
    }

    private void TryAddMoney(long newMoney) {
        try {
            checked {
                PlayerCapital += newMoney;
                if (!_tutorialCompiller.TutorialPassed && PlayerCapital > _maxTutorialAmount) {
                    PlayerCapital = _maxTutorialAmount;
                }
                else {
                    BankNewMoneyPlus?.Invoke(newMoney);
                }
            }
        }
        catch (OverflowException) {
            PlayerCapital = long.MaxValue;
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
