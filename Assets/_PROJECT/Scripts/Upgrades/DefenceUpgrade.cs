using System;
using UnityEngine;

public class DefenceUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateDefence((int)_k);
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.DefenceCount);
        
        _currentPrice *= _priceMultiply;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(_level, _playerStats.DefenceCount, _playerStats.DefenceCount+_k, _currentPrice);
    }
    
}
