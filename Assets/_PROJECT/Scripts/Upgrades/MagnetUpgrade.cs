using System;
using UnityEngine;

public class MagnetUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        Debug.Log("Покупка MagniteUpgrade: " + _playerStats.MagnetLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;
        _playerStats.UpdateMagnetLevel(_level);

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level, 
            _upgradesCalculator.GetMagnetKByLevel(), 
            _upgradesCalculator.GetMagnetKByLevel(false), 
            _currentPrice,
            "",
            false);
        UpdateLevelInLeft(_level);
        
    }

    protected override void LoadLevel() {
        UpgradeInfo = _config.MagneteUpgrade;
        _level = _gameSave.GetSave.GetUpgradeLevel(UpgradeInfo.Id);
        _playerStats.UpdateMagnetLevel(_level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, _level);
        UpdateVisual();
    }
}
