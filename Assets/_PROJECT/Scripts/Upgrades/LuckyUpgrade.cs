

using UnityEngine;

public class LuckyUpgrade : UpgradeBase {

    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateLucky(_k);
        Debug.Log("Покупка LuckyUpgrade: " + _playerStats.LuckyMultiplier);
        
        _currentPrice *= _priceMultiply;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
    protected override void UpdateVisual() {
        _visual.UpdateData(_level, _playerStats.LuckyMultiplier, _playerStats.LuckyMultiplier*_k, _currentPrice);
    }

}
