using System;
using UnityEngine;

public class MagnetUpgrade : UpgradeBase {
    public override void LoadLevel() {
        _upgradeInfo = _config.MagneteUpgrade;
        _playerStats.UpdateMagnetLevel(Level, false);
        
        _visual.SetNameText(UpgradeType);
        UpdatePrice();
        UpdateVisual();
    }
 

    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка MagniteUpgrade: " + _playerStats.MagnetLevel);
        _playerStats.UpdateMagnetLevel(Level);
    }

    protected override void UpdatePrice() {
        _currentPrice = 120 * Mathf.Pow(Level+1, 3);
    }

    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetMagnetKByLevel(), 
            _upgradesCalculator.GetMagnetKByLevel(false), 
            _currentPrice,
            _localization.MagnetPower,
            false);
        _visual.UpdateLevelInLeft(Level);
        
    }
    
}
