using UnityEngine;

public class LuckyUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        
        Debug.Log("Покупка LuckyUpgrade: " + _playerStats.LuckyLevel);
        
        
        
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;
        _playerStats.UpdateLuckyLevel(_level);

        UpdateVisual();
        CheckColor();
    }
    
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level,
            _upgradesCalculator.GetLuckyByLevel(), 
            _upgradesCalculator.GetLuckyByLevel(false), 
            _currentPrice,  
            "м", 
            false);
    }

    protected override void LoadLevel() {
        UpgradeInfo = _config.LuckyUpgrade;
        _level = _gameSave.GetSave.GetUpgradeLevel(UpgradeInfo.Id);
        _playerStats.UpdateLuckyLevel(_level, false);
        
        _visual.SetNameText(_localization.GetUpgradeName(UpgradeType));
        _currentPrice = UpgradeInfo.StartPrice * Mathf.Pow(UpgradeInfo.PriceMultiplier, _level);
        UpdateVisual();
    }
}
