using System;
using UnityEngine;

public class PlayerStats : IPlayerStatsReadOnly, IPlayerStatsWritable {
    public int MultiplierLevel { get; private set; } = 1;
    public int LuckyLevel { get; private set; } = 1;
    public int MagnetLevel { get; private set; } = 1;
    public int DefenceLevel { get; private set; } = 1;
    public int PredictDistanceLevel { get; private set; } = 1;
    public event Action ChangeStats;

    public void UpdateMultiplierLevel(int x, bool isInvokable = true) {
        MultiplierLevel += x;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateLuckyLevel(int x, bool isInvokable = true) {
        LuckyLevel += x;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateMagnetLevel(int x, bool isInvokable = true) {
        MagnetLevel += x;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdateDefenceLevel(int x, bool isInvokable = true) {
        DefenceLevel += x;
        if (!isInvokable) {
            return;
        }
        ChangeStats?.Invoke();
    }

    public void UpdatePredictDistanceLevel(int x, bool isInvokable = true) {
        PredictDistanceLevel += x;
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
    void UpdateMultiplierLevel(int x = 1, bool isInvokable = true);
    void UpdateLuckyLevel(int x = 1, bool isInvokable = true);
    void UpdateMagnetLevel(int x = 1, bool isInvokable = true);
    void UpdateDefenceLevel(int x = 1, bool isInvokable = true);
    void UpdatePredictDistanceLevel(int x = 1, bool isInvokable = true);
}
