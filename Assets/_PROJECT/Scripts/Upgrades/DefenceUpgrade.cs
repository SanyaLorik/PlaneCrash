using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    public override void LoadLevel() {
        _upgradeInfo = _config.DefenceUpgrade;
        _visual.SetNameText(UpgradeType);
        _playerStats.UpdateDefenceLevel(Level, false);
        UpdatePrice();
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.DefenceLevel);
        _playerStats.UpdateDefenceLevel(Level);
    }

    protected override void UpdatePrice() {
        if (Level == 1) {
            _currentPrice = UpgradeInfo.StartPrice;
        }
        else {
            // 100 * будущий_уровень^6
            _currentPrice = 100 * Mathf.Pow(Level+1, 6);
        }
    }


    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetDefenceByLevel(), 
            _upgradesCalculator.GetDefenceByLevel(false), 
            _currentPrice,
            _localization.Pieces,
            true);
        _visual.UpdateLevelInLeft(Level);

    }
    
}
