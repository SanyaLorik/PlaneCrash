using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class XMultiplyUpgrade : MonoBehaviour {
    [Header("Рост каждый раз в К раз больше")]
    [SerializeField] private float _k;
    [Header("Рост цены")]
    [SerializeField] private float _priceMultiply;
    
    [SerializeField] private float _startPrice;

    private int _level = 1;
    private float _currentPrice;
    private UpgradeItemVisual _visual;
    private IPlayerStatsWritable _playerStats;

    private PlayerBank _bank;

    [Inject]
    public void Init(PlayerBank bank, IPlayerStatsWritable playerStats) {
        _playerStats = playerStats;
        _bank = bank;
        _bank.BankChanged += BankOnBankChanged;
    }

    private void Awake() {
        _visual = GetComponent<UpgradeItemVisual>();
        _currentPrice = _startPrice;
        _visual.UpdateData(_level, _playerStats.XMultiplier, _playerStats.XMultiplier*_k, _currentPrice);
    }

    private void BankOnBankChanged(float playerCapital) {
        CheckColor();
    }

    private void CheckColor() {
        if (_bank.CanBuy(_currentPrice)) {
            _visual.SetGreen();
            return;
        }
        _visual.SetRed();
    }
    
    

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.TryGetComponent(out PlayerMovement _)) {
            if (_bank.CanBuy(_currentPrice)) {
                BuyUpgrade();
            }
            else {
                Debug.LogWarning("Не хватает срэдств(");
            }
        }
    }

    private void BuyUpgrade() {
        Debug.Log("Покупка");
        _bank.Buy(_currentPrice);

        _playerStats.MultiplyXMultiplier(_k);
        _currentPrice *= _priceMultiply;
        _level++;
        _visual.UpdateData(_level, _playerStats.XMultiplier, _playerStats.XMultiplier*_k, _currentPrice);
        CheckColor();
    }
    
    
    
}
