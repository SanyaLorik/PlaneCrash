using UnityEngine;
using Zenject;

/// <summary>
/// Преобразование уровня апгрейда в число 
/// </summary>
public class UpgradesCalculator {
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private UpgradeConfig _config;
    [Inject] private PetsManager _petsManager;


    [Inject]
    private void Init(PetsManager petsManager) {
        _petsManager = petsManager;
        petsManager.BuyPet += PetsManagerOnBuyPet;
    }

    
    private void PetsManagerOnBuyPet() {
        _needRecalculate = true;
    }


    public float GetLuckyByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel-1);
        }
        return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel);
    }


    public float GetUpgradeMultiplierByLevel(bool thisLevel = true, bool forceUpdate = false) {
        if (thisLevel) { 
            return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.MultiplierLevel-1) * GetPetMultiplier(forceUpdate);
        }
        return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.MultiplierLevel) * GetPetMultiplier(forceUpdate);
    } 
    

    
    // Пока прст экспоненциально
    public float GetMagnetKByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.MagneteUpgrade.BaseValue * Mathf.Pow(_config.MagneteUpgrade.K, _playerStats.MagnetLevel - 1);
        }

        return _config.MagneteUpgrade.BaseValue * Mathf.Pow(_config.MagneteUpgrade.K, _playerStats.MagnetLevel);
    }
    
    
    public Vector3 GetMagnetSizeByLevel(Vector3 minSize, Vector3  maxSize) {
        int level = _playerStats.MagnetLevel;
        float speed = _config.MagneteSizeGrowSpeed; // например 0.12f
        // Какаято невьебическая формула t = 1 - e^(-level * speed)
        float t = 1f - Mathf.Exp(-level * speed);
        return Vector3.Lerp(minSize, maxSize, t);
    }

    public float GetPredictDistanceByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.PredictionUpgrade.BaseValue + (_config.PredictionUpgrade.K * (_playerStats.PredictDistanceLevel-1));
        }
        return _config.PredictionUpgrade.BaseValue + (_config.PredictionUpgrade.K * _playerStats.PredictDistanceLevel);
    }


    public int GetDefenceByLevel(bool thisLevel = true) {
        // Тут сила равна level - 1
        if (thisLevel) {
            return _playerStats.DefenceLevel-1;
        }

        return _playerStats.DefenceLevel;
    }

    private bool _needRecalculate = true;
    private float _petMultiplier;
    private float GetPetMultiplier(bool forceUpdate = false) {
        if (!_needRecalculate && !forceUpdate) {
            return _petMultiplier;
        }
        _petMultiplier = 1f;
        foreach (var pet in _petsManager.PetsInstances) {
            _petMultiplier *= pet.PetInfo.Modifier;
        }
        Debug.Log($"{_petsManager.PetsInstances.Count} pets multiplayer = {_petMultiplier}");
        _needRecalculate = false;
        return _petMultiplier;
    }


}
