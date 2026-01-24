using System;
using UnityEngine;

public class MagnetUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdateMagnet((int)_k);
        Debug.Log("Покупка MagniteUpgrade: " + _playerStats.MagnetSpeed);
        
        _currentPrice *= _priceMultiply;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(_level, _playerStats.MagnetSpeed, _playerStats.MagnetSpeed+_k, _currentPrice);
    }
    
}
