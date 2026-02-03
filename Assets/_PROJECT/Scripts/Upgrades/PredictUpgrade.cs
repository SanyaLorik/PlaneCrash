using System;
using UnityEngine;

public class PredictUpgrade : UpgradeBase {
    
    private void Start() {
        UpgradeInfo = _config.PredictionUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        UpdateVisual();
    }
    
    
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdatePredictDistanceLevel();
        Debug.Log("Покупка PredictUpgrade: " + _playerStats.PredictDistanceLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level, 
            _upgradesCalculator.GetPredictDistanceByLevel(), 
            _upgradesCalculator.GetPredictDistanceByLevel(false), 
            _currentPrice,
            "м",
            true);
    }
    
}
