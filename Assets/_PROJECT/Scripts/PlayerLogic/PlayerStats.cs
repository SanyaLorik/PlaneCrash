using System;
using UnityEngine;
using Zenject;

public class PlayerStats : IPlayerStatsReadOnly, IPlayerStatsWritable {

    public int MultiplierLevel { get; private set; } = 1;
    public int LuckyLevel { get; private set; } = 1;
    public int MagnetLevel { get; private set; } = 1;
    public int DefenceLevel { get; private set;} = 1;
    public int PredictDistanceLevel { get; private set; } = 1;
    public event Action ChangeStats;
    
    
    public void UpdateMultiplierLevel(int level, bool isInvokable = true) {
        MultiplierLevel = level;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateLuckyLevel(int level, bool isInvokable = true) {
        LuckyLevel = level;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateMagnetLevel(int level, bool isInvokable = true) {
        MagnetLevel = level;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateDefenceLevel(int level, bool isInvokable = true) {
        DefenceLevel = level;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdatePredictDistanceLevel(int level, bool isInvokable = true) {
        PredictDistanceLevel = level;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }
}

public interface IPlayerStatsReadOnly {
   int MultiplierLevel { get; }
   int LuckyLevel { get; }
   int MagnetLevel { get; }
   int DefenceLevel { get; }
   int PredictDistanceLevel { get; }
    
    public event Action ChangeStats;
}

public interface IPlayerStatsWritable {
    int MultiplierLevel { get; }
    int LuckyLevel { get; }
    int MagnetLevel { get; }
    int DefenceLevel { get; }
    int PredictDistanceLevel { get; }
    void UpdateMultiplierLevel(int level, bool isInvokable = true);
    void UpdateLuckyLevel(int level, bool isInvokable = true);
    void UpdateMagnetLevel(int level, bool isInvokable = true);
    void UpdateDefenceLevel(int level, bool isInvokable = true);
    void UpdatePredictDistanceLevel(int level, bool isInvokable = true);
}
