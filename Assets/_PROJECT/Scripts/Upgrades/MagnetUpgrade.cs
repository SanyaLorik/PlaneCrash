using System;
using UnityEngine;

public class MagnetUpgrade : UpgradeBase {
    public override void LoadLevel() {
        _upgradeInfo = _config.MagneteUpgrade;
        _playerStats.UpdateMagnetLevel(Level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, Level);
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка MagniteUpgrade: " + _playerStats.MagnetLevel);
        _playerStats.UpdateMagnetLevel(Level);
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetMagnetKByLevel(), 
            _upgradesCalculator.GetMagnetKByLevel(false), 
            _currentPrice,
            "",
            false);
        UpdateLevelInLeft();
        
    }
    
}
