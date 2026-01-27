using System;
using UnityEngine;

public class PredictUpgrade : UpgradeBase {
    
    protected override void ApplyUpgrade() {
        _bank.Buy(_currentPrice);
        _playerStats.UpdatePredictDistance((int)_k);
        Debug.Log("Покупка DefenceUpgrade: " + _playerStats.PredictDistance);
        
        _currentPrice *= _priceMultiply;
        _level++;

        UpdateVisual();
        CheckColor();
    }
    
        
    protected override void UpdateVisual() {
        _visual.UpdateData(_level, _playerStats.PredictDistance, _playerStats.PredictDistance+_k, _currentPrice);
    }
    
}
