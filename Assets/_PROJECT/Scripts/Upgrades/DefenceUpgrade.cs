using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    public override void LoadLevel() {
        _upgradeInfo = _config.DefenceUpgrade;
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _playerStats.UpdateDefenceLevel(Level, false);
        UpdatePrice();
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.DefenceLevel);
        _playerStats.UpdateDefenceLevel(Level);
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetDefenceByLevel(), 
            _upgradesCalculator.GetDefenceByLevel(false), 
            _currentPrice,
            "шт",
            true);
        UpdateLevelInLeft();
    }
    
}
