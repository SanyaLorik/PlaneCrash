using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class XMultiplyUpgrade : UpgradeBase {

    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateXMultiplier(_k);
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.XMultiplier);
        
        
        _currentPrice *= _priceMultiply;
        _level++;


        UpdateVisual();
        CheckColor();
    }

    protected override void UpdateVisual() {
        _visual.UpdateData(_level, _playerStats.XMultiplier, _playerStats.XMultiplier*_k, _currentPrice);
    }
}
