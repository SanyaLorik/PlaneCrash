using System;
using TMPro;
using UnityEngine;

public class PlayerBank : MonoBehaviour {
    
    [SerializeField] private float _playerCapital;
    [SerializeField] private TMP_Text _playerCapitalVisual;
    public static PlayerBank Instance { get; private set; }
    public event Action<float> OnBankChanged;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("PlayerBank Instance already exists, destroying object");
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
    
    private void Start() {
        OnBankChanged?.Invoke(_playerCapital);
    }
    
}
