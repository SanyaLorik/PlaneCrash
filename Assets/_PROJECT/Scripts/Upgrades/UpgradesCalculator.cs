using System;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Zenject;

/// <summary>
/// Преобразование уровня апгрейда в число 
/// </summary>
public class UpgradesCalculator {
    private PetsManager _petsManager;
    private bool _needRecalculate = true;
    private float _petMultiplier;
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private UpgradeConfig _config;
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private ZoneManager _zoneManager;


    [Inject]
    private void Init(PetsManager petsManager) {
        _petsManager = petsManager;
        petsManager.GetPet += PetsManagerOnGetPet;
    }

    
    private void PetsManagerOnGetPet() {
        _needRecalculate = true;
    }


    public float GetLuckyByLevel(bool thisLevel = true) {
        int level = thisLevel ? _playerStats.LuckyLevel - 1 : _playerStats.LuckyLevel;
        return _config.LuckyUpgrade.BaseValue + 2 * level;
    }


    public float GetUpgradeMultiplierByLevel(bool thisLevel = true, bool forceUpdate = false) {
        int level = thisLevel ? _playerStats.MultiplierLevel - 1 : _playerStats.MultiplierLevel;
        return _config.XMultiplierUpgrade.BaseValue + level * 0.05f
                + 
                GetPetMultiplier(forceUpdate);
        
    } 
    

    // Пока прст экспоненциально
    public float GetMagnetKByLevel(bool thisLevel = true) {
        int level = thisLevel ? _playerStats.MagnetLevel - 1 : _playerStats.MagnetLevel;
        return _config.MagneteUpgrade.BaseValue * Mathf.Pow(_config.MagnetLevelK, level);
    }
    
    
    public Vector3 GetMagnetSizeByLevel(Vector3 minSize, Vector3  maxSize) {
        int level = _playerStats.MagnetLevel;
        float speed = _config.MagneteSizeGrowSpeed; // например 0.12f
        // Какаято невьебическая формула t = 1 - e^(-level * speed)
        float t = 1f - Mathf.Exp(-level * speed);
        return Vector3.Lerp(minSize, maxSize, t);
    }

    public float GetPredictDistanceByLevel(bool thisLevel = true) {
        int level = thisLevel ? _playerStats.PredictDistanceLevel - 1 : _playerStats.PredictDistanceLevel;
        return _config.PredictionUpgrade.BaseValue + 5 * level;
    }


    public int GetDefenceByLevel(bool thisLevel = true) {
        // Тут сила равна level - 1
        if (thisLevel) {
            return _playerStats.DefenceLevel-1;
        }

        return _playerStats.DefenceLevel;
    }
    
    
    
    public long GetWinMoney() {
        double result = (double)GetUpgradeMultiplierByLevel() 
                        * _zoneManager.BetMultiplier 
                        + _playerStateManager.CurrentPlayerDistance();
    
        if (result > long.MaxValue) return long.MaxValue;
        if (result < long.MinValue) return long.MinValue; // хотя минус вряд ли
    
        return (long)result;
    }
    
    public float GetDistanceMoney() {
        double result = (double)GetUpgradeMultiplierByLevel() 
                        *
                        _playerStateManager.CurrentPlayerDistance();
    
        if (result > long.MaxValue) return long.MaxValue;
        if (result < long.MinValue) return long.MinValue; // хотя минус вряд ли
        
        
        return (long)result;
    }
    
    
    

    private float GetPetMultiplier(bool forceUpdate = false) {
        if (!_needRecalculate && !forceUpdate) {
            return _petMultiplier;
        }

        _petMultiplier = 0;
        foreach (var pet in _petsManager.PetsInstances) {
            _petMultiplier += pet.PetInfo.Modifier;
        }
        Debug.Log($"{_petsManager.PetsInstances.Count} pets multiplier = {_petMultiplier}");
        _needRecalculate = false;
        return _petMultiplier;
    }


}
