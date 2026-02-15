using UnityEngine;
using Zenject;

/// <summary>
/// Преобразование уровня апгрейда в число 
/// </summary>
public class UpgradesCalculator {
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private UpgradeConfig _config;
    [Inject] private PetsManager _petsManager;
    

    public float GetLuckyByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel-1);
        }
        return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel);
    }


    public float GetUpgradeMultiplierByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.MultiplierLevel-1) * GetPetMultiplyer();;
        }
        return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.MultiplierLevel) * GetPetMultiplyer();;
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

    private float GetPetMultiplyer() {
        float multiplayer = 1f;
        foreach (var pet in _petsManager.PetsInstances) {
            multiplayer *= pet.PetInfo.Modifier;
        }
        Debug.Log($"{_petsManager.PetsInstances.Count} pets multiplayer = {multiplayer}");
        return multiplayer;
    }


}
