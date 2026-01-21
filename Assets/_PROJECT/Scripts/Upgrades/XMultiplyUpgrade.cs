using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class XMultiplyUpgrade : UpgradeBase {
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.MultiplyXMultiplier(_k);
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.XMultiplier);
        
        
        _currentPrice *= _priceMultiply;
        _level++;
        

        _visual.UpdateData(_level, _playerStats.XMultiplier, _playerStats.XMultiplier*_k, _currentPrice);
        CheckColor();
    }
}
