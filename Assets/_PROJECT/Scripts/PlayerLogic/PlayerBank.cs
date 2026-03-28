using System;
using Architecture_M;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    [SerializeField] private MoneyCube _cube;
    [SerializeField] private long _maxTutorialAmount;
    [SerializeField] private TextMeshProUGUI _playerCapitalText;
    
    
    public event Action<long> BankChanged;
    public event Action<long> BankNewMoneyPlus;
    public event Action<long> BankNewMoneyMinus;
    public event Action<long> MoneyCollect;

    [Inject] IGameSave<GameSavePC> _gameSave;
    [Inject] TutorialCompiller _tutorialCompiller;
    [Inject] PlayerConfig _playerConfig;
    [Inject] NumberFormatter _formatter;


    private void Start() {
        if (PlayerCapital < _playerConfig.StartMoneyAmount) {
            AddMoney(_playerConfig.StartMoneyAmount);
        }
        // Просто чтоб в визуале отобразить
        PlayerCapital = _gameSave.GetSave.Money;
    }


    public long PlayerCapital {
        get => _gameSave.GetSave.Money;
        private set {
            _playerCapitalText.text = _formatter.ValuteFormatter(value); 
            _gameSave.GetSave.Money = value;
        } 
    }
    public long PlayerRecord {
        get => _gameSave.GetSave.RecordMoney; 
        private set => _gameSave.GetSave.RecordMoney = value;
    }

    private void ChangeMoney(long newMoney, bool save = true) {
        if (newMoney == long.MinValue) {
            Debug.LogWarning("Попытка передать long.MinValue");
            return;
        }
        
        
        if (newMoney > 0) {
            TryAddMoney(newMoney);
        }
        else {
            // Для отрицательных значений проверяем, что не уйдём в минус
            if (PlayerCapital + newMoney < 0) {
                PlayerCapital = 0;
            }
            else {
                PlayerCapital += newMoney;
            }
            BankNewMoneyMinus?.Invoke(Math.Abs(newMoney));
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
        // Защита от передачи отрицательных или нулевых значений
        if (newMoney <= 0) return;
    
        try {
            checked {
                // Проверяем, что добавление не вызовет переполнение
                if (PlayerCapital > long.MaxValue - newMoney) {
                    PlayerCapital = long.MaxValue;
                }
                else {
                    PlayerCapital += newMoney;
                }
            
                if (!_tutorialCompiller.TutorialPassed && PlayerCapital > _maxTutorialAmount) {
                    PlayerCapital = _maxTutorialAmount;
                }
                else if (newMoney > 0) {
                    BankNewMoneyPlus?.Invoke(newMoney);
                }
            }
        }
        catch (OverflowException) {
            PlayerCapital = long.MaxValue;
        }
    }

    public void BankActionInvoke() {
        BankChanged?.Invoke(PlayerCapital);
    }


    public void Buy(double amount) {
        if (!CanBuy(amount)) return;
    
        // Защита от переполнения double -> long
        long longAmount;
        try {
            longAmount = (long)amount;
        }
        catch (OverflowException) {
            return; // Слишком большое число для long
        }
    
        ChangeMoney(-longAmount);
    }
    
    public void AddMoney(double amount) {
        if (amount <= 0) return;
    
        // Защита от переполнения double -> long
        long longAmount;
        try {
            longAmount = (long)amount;
        }
        catch (OverflowException) {
            longAmount = long.MaxValue;
        }
    
        ChangeMoney(longAmount);
    }

    public void AddFlightMoney(long amount) {
        if (amount <= 0) return;
        MoneyCollect?.Invoke(amount);
        TryAddMoney(amount);
    }


    public bool CanBuy(double amount) =>
        PlayerCapital >= amount;


}
