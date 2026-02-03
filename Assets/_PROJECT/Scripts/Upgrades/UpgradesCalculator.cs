using UnityEngine;
using Zenject;

/// <summary>
/// Преобразование уровня апгрейда в число 
/// </summary>
public class UpgradesCalculator {
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private UpgradeConfig _config;




    public float GetLuckyByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel);
        }
        return _config.LuckyUpgrade.BaseValue * Mathf.Pow(_config.LuckyUpgrade.K,_playerStats.LuckyLevel  + 1);

    }


    public float GetXMultiplierByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.XMultiplierLevel);
        }
        return _config.XMultiplierUpgrade.BaseValue * Mathf.Pow(_config.XMultiplierUpgrade.K,_playerStats.XMultiplierLevel + 1);
    } 
    

    
    // Пока прст экспоненциально
    public float GetMagnetKByLevel(bool thisLevel = true) {
        if (thisLevel) {
            return _config.MagneteUpgrade.BaseValue * Mathf.Pow(_config.MagneteUpgrade.K,_playerStats.MagnetLevel);
        }
        return _config.MagneteUpgrade.BaseValue * Mathf.Pow(_config.MagneteUpgrade.K,_playerStats.MagnetLevel + 1);
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
            return _config.PredictionUpgrade.BaseValue + (_config.PredictionUpgrade.K * _playerStats.PredictDistanceLevel);
        }
        return _config.PredictionUpgrade.BaseValue + (_config.PredictionUpgrade.K * (_playerStats.PredictDistanceLevel + 1));
    }


    public int GetDefenceByLevel(bool thisLevel = true) {
        // Тут сила равна level - 1
        if (thisLevel) {
            return _playerStats.DefenceLevel - 1;
        }
        return _playerStats.DefenceLevel;
    }


}
