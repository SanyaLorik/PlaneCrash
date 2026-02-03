

using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class LuckyUpgrade : UpgradeBase {


    private void Start() {
        UpgradeInfo = _config.LuckyUpgrade;
        _currentPrice = UpgradeInfo.StartPrice;
        UpdateVisual();
    }
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        
        _playerStats.UpdateLuckyLevel();
        Debug.Log("Покупка LuckyUpgrade: " + _playerStats.LuckyLevel);
        
        
        
        
        _currentPrice *= UpgradeInfo.PriceMultiplier;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
    protected override void UpdateVisual() {
        _visual.UpdateData(
            _level,
            _upgradesCalculator.GetLuckyByLevel(), 
            _upgradesCalculator.GetLuckyByLevel(false), 
            _currentPrice,  
            "", 
            false);
    }

}
