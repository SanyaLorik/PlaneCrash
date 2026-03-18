using UnityEngine;

public class PredictUpgrade : UpgradeBase {
    
    public override void LoadLevel() {
        _upgradeInfo = _config.PredictionUpgrade;
        _playerStats.UpdatePredictDistanceLevel(Level, false);
        _visual.SetNameText(UpgradeType);
        
        UpdatePrice();
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка PredictUpgrade: " + _playerStats.PredictDistanceLevel);
        _playerStats.UpdatePredictDistanceLevel(Level);
    }

    protected override void UpdatePrice() {
        _currentPrice = 150 * Mathf.Pow(Level+1, 4);
    }

    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetPredictDistanceByLevel(), 
            _upgradesCalculator.GetPredictDistanceByLevel(false), 
            _currentPrice,
            _localization.Meters,
            true);
        _visual.UpdateLevelInLeft(Level);

        
    }
    
}
