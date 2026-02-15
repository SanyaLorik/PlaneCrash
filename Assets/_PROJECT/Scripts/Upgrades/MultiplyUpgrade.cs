using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class MultiplyUpgrade : UpgradeBase {
    
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.MultiplierLevel);
        
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;   
        _level++;
        _playerStats.UpdateMultiplierLevel(_level);


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
        _level = _gameSave.GetSave.GetUpgradeLevel(UpgradeInfo.Id);
        _playerStats.UpdateMultiplierLevel(_level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, _level);
        UpdateVisual();
    }
}
