using System;
using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.PredictDistanceLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;
        _playerStats.UpdateDefenceLevel(_level);

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
        _level = _gameSave.GetSave.GetUpgradeLevel(UpgradeInfo.Id);
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, _level);
        _playerStats.UpdateDefenceLevel(_level, false);
        UpdateVisual();
    }
}
