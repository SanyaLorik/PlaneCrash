using System;
using UnityEngine;

public class MagnetUpgrade : UpgradeBase {
    
    private void Start() {
        UpgradeInfo = _config.MagneteUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        UpdateVisual();
    }
    
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateMagnetLevel();
        Debug.Log("Покупка MagniteUpgrade: " + _playerStats.MagnetLevel);
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;

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
    }
    
}
