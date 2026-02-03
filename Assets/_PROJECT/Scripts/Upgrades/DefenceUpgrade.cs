using System;
using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    
    private void Start() {
        UpgradeInfo = _config.DefenceUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        UpdateVisual();
    }
    
    
    
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
    
}
