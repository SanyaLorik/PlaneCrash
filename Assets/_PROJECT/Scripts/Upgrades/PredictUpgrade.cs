using System;
using UnityEngine;

public class PredictUpgrade : UpgradeBase {

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

    protected override void LoadLevel() {
        UpgradeInfo = _config.PredictionUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        _playerStats.UpdatePredictDistanceLevel(_level, false);
        UpdateVisual();
    }
}
