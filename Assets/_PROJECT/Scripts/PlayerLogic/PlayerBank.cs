using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    
    [SerializeField] private TMP_Text _playerCapitalVisual;
    [SerializeField] private MoneyCube _cube;
    private float _playerCapital;
    
    
    public event Action<float> BankChanged;
    public event Action<float> MoneyCollect;
    public float PlayerCapital { get => _playerCapital;  }

    [Inject] IGameSave<GameSavePC> _gameSave;
    
    private void Start() {
        BankChanged += OnBankChanged;
        LoadPlayerMoney();
    }

    
    private void LoadPlayerMoney() {
        _playerCapital = _gameSave.GetSave.Money;
        BankChanged?.Invoke(PlayerCapital);
    }
    
 

    private void OnBankChanged(float obj) {
        _cube.SetMoneyAmount(_playerCapital);
        _gameSave.GetSave.Money = (long)obj;
        _gameSave.Save();
    }



    


    public void AddMoney(float amount) {
        if (amount < 0) return;
        _playerCapital += amount;
        BankChanged?.Invoke(_playerCapital);
    }
    
    
    public void AddFlightMoney(float amount) {
        if (amount < 0) return;
        _playerCapital += amount;
        MoneyCollect?.Invoke(amount);
    }
    
    
    
    
    public void Buy(float amount) {
        if (amount > _playerCapital) return;
        _playerCapital -= amount;
        BankChanged?.Invoke(_playerCapital);
    }


    public bool CanBuy(float amount) =>
        _playerCapital >= amount;
    
    public void GiveMeYourFuckingMoneyNigga(float amount) {
        if (amount > _playerCapital) {
            Debug.LogWarning("Как ты сука поставил денег больше чем у тебя было");
            _playerCapital = 0;
        }
        _playerCapital -= amount;
        BankChanged?.Invoke(_playerCapital);
    }
    

}
