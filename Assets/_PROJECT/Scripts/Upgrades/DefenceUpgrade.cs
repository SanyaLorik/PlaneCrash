using System;
using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateDefenceLevel();
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.PredictDistanceLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level, 
            _upgradesCalculator.GetDefenceByLevel(), 
            _upgradesCalculator.GetDefenceByLevel(false), 
            _currentPrice,
            "шт",
            true);
        
    }

    protected override void LoadLevel() {
        UpgradeInfo = _config.DefenceUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        _playerStats.UpdateDefenceLevel(_level, false);
        UpdateVisual();
    }
}
