using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class XMultiplyUpgrade : UpgradeBase {
    
    private void Start() {
        UpgradeInfo = _config.XMultiplierUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        UpdateVisual();
    }
    
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateXMultiplierLevel();
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.XMultiplierLevel);
        
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;


        UpdateVisual();
        CheckColor();
    }

    
    
    
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level, 
            _upgradesCalculator.GetXMultiplierByLevel(), 
            _upgradesCalculator.GetXMultiplierByLevel(false), 
            _currentPrice,  
            "x", 
            false);
    }
}
