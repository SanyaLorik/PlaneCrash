using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class MultiplyUpgrade : UpgradeBase {
    
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateMultiplierLevel();
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.MultiplierLevel);
        
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;


        UpdateVisual();
        CheckColor();
    }

    
    
    
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level, 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(), 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(false), 
            _currentPrice,  
            "x", 
            false);
    }

    protected override void LoadLevel() {
        UpgradeInfo = _config.XMultiplierUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        _playerStats.UpdateMultiplierLevel(_level, false);
        UpdateVisual();
    }
}
