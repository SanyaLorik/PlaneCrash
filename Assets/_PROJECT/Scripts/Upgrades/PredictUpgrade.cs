using System;
using UnityEngine;

public class PredictUpgrade : UpgradeBase {

    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        Debug.Log("Покупка PredictUpgrade: " + _playerStats.PredictDistanceLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;
        _playerStats.UpdatePredictDistanceLevel(_level);

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
        _level = _gameSave.GetSave.GetUpgradeLevel(UpgradeInfo.Id);
        _playerStats.UpdatePredictDistanceLevel(_level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, _level);
        UpdateVisual();
    }
    
}
