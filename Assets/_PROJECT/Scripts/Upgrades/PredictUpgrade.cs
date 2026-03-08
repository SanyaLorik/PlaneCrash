using UnityEngine;

public class PredictUpgrade : UpgradeBase {
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка PredictUpgrade: " + _playerStats.PredictDistanceLevel);
        _playerStats.UpdatePredictDistanceLevel(Level);
    }
        
    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetPredictDistanceByLevel(), 
            _upgradesCalculator.GetPredictDistanceByLevel(false), 
            _currentPrice,
            "м",
            true);
        UpdateLevelInLeft();
        
    }

    public override void LoadLevel() {
        _upgradeInfo = _config.PredictionUpgrade;
        _playerStats.UpdatePredictDistanceLevel(Level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, Level);
        UpdateVisual();
    }
    
}
