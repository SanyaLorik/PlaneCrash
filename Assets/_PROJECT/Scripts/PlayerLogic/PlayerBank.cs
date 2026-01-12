using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerBank : MonoBehaviour {
    
    [SerializeField] private float _playerCapital;
    [SerializeField] private TMP_Text _playerCapitalVisual;
    [SerializeField] private MoneyCube _cube;
    public event Action<float> OnBankChanged;

    
    private void Start() {
        OnBankChanged += BankChanged;
        LoadPlayerMoney().Forget();
    }

    private void BankChanged(float obj) {
        _cube.SetMoneyAmount(_playerCapital);
    }

    private async UniTaskVoid LoadPlayerMoney() {
        // Имитация задержки перед загрузкой денег
        await UniTask.Delay(1000);
        OnBankChanged?.Invoke(_playerCapital);
    }

    

    public float PlayerCapital { get => _playerCapital; }

    public void AddMoney(float amount) {
        if (amount < 0) return;
        _playerCapital += amount;
        OnBankChanged?.Invoke(_playerCapital);
    }
    
    public void GiveMeYourFuckingMoneyNigga(float amount) {
        if (amount > _playerCapital) {
            Debug.LogWarning("Как ты сука поставил денег больше чем у тебя было");
            _playerCapital = 0;
        }
        _playerCapital -= amount;
        OnBankChanged?.Invoke(_playerCapital);
    }
    

}
