using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerBank : MonoBehaviour {
    
    [SerializeField] private float _playerCapital;
    [SerializeField] private TMP_Text _playerCapitalVisual;
    [SerializeField] private MoneyCube _cube;
    public event Action<float> BankChanged;
    public event Action<float> MoneyCollect;
    public float PlayerCapital { get => _playerCapital;  }

    [Inject] IGameSave<GameSavePC> _gameSave;
    
    private void Start() {
        BankChanged += OnBankChanged;
        LoadPlayerMoney().Forget();
    }

    [Header("Для теста")]
    [SerializeField] private bool _updateMoney = false;
    // Сугубо для теста 
    private void Update() {
        if (_updateMoney) {
            BankChanged?.Invoke(_playerCapital);
            _updateMoney = false;
        }
    }

    private void OnBankChanged(float obj) {
        _cube.SetMoneyAmount(_playerCapital);
        _gameSave.GetSave.Money = (long)obj;
        _gameSave.Save();
    }

    private async UniTaskVoid LoadPlayerMoney() {
        // Имитация задержки перед загрузкой денег
        await UniTask.Delay(500);
        _playerCapital = _gameSave.GetSave.Money;
        BankChanged?.Invoke(PlayerCapital);
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
