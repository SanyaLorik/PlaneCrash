using System;
using UnityEngine;
using Zenject;

public class MultiplyUpgrade : UpgradeBase {

    [Inject] private PetsManager _petsManager;
    
    private void OnEnable() {
        _petsManager.GetPet += PetsManagerOnGetPet;
    }

    private void PetsManagerOnGetPet() {
        UpdateVisual();
    }


    public override void LoadLevel() {
        _upgradeInfo = _config.XMultiplierUpgrade;
        _playerStats.UpdateMultiplierLevel(Level, false);
        _visual.SetNameText(UpgradeType);
        
        UpdatePrice();
        UpdateVisual();
    }
    
    protected override void UpdatePlayerStatsInfo() {
        Debug.Log("Покупка XMultiplyUpgrade: " + _playerStats.MultiplierLevel);
        _playerStats.UpdateMultiplierLevel(Level);
    }

    protected override void UpdatePrice() {
        _currentPrice = 15 * 10 * Mathf.Pow(Level+1, 4.2f);
    }

    protected override void UpdateVisual() {
        _visual.UpdateData(
            Level, 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(), 
            _upgradesCalculator.GetUpgradeMultiplierByLevel(false), 
            _currentPrice,  
            _localization.Xs, 
            false);
        _visual.UpdateLevelInLeft(Level);

    }

    
}
