using UnityEngine;

public class MultiplyUpgrade : UpgradeBase {
    
    public override void LoadLevel() {
        _upgradeInfo = _config.XMultiplierUpgrade;
        _playerStats.UpdateMultiplierLevel(Level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, Level);
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.MultiplierLevel);
        _playerStats.UpdateMultiplierLevel(Level);
    }

    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(), 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(false), 
            _currentPrice,  
            "x", 
            false);
        UpdateLevelInLeft();
    }

    
}
