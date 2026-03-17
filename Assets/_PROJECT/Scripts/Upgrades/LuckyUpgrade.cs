using UnityEngine;

public class LuckyUpgrade : UpgradeBase {
    public override void LoadLevel() {
        _upgradeInfo = _config.LuckyUpgrade;
        _playerStats.UpdateLuckyLevel(Level, false);
        _visual.SetNameText(UpgradeType);
        
        UpdatePrice();
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка LuckyUpgrade: " + _playerStats.LuckyLevel);
        _playerStats.UpdateLuckyLevel(Level);
    }

    protected override void UpdatePrice() {
        _currentPrice = 70 * Mathf.Pow(Level+1, 4);
    }


    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level,
            _upgradesCalculator.GetLuckyByLevel(), 
            _upgradesCalculator.GetLuckyByLevel(false), 
            _currentPrice,  
            _localization.SecondTime, 
            false);
        _visual.UpdateLevelInLeft(Level);

    }


}
