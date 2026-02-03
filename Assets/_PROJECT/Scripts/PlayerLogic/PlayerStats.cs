using System;
using UnityEngine;

public class PlayerStats : IPlayerStatsReadOnly, IPlayerStatsWritable {
    public int XMultiplierLevel { get; private set; } = 1;
    public int LuckyLevel { get; private set; } = 1;
    public int MagnetLevel { get; private set; } = 1;
    public int DefenceLevel { get; private set; } = 1;
    public int PredictDistanceLevel { get; private set; } = 1;
    public event Action ChangeStats;

    public void UpdateXMultiplierLevel(int x) {
        XMultiplierLevel += x;
        ChangeStats?.Invoke();
    }

    public void UpdateLuckyLevel(int x) {
        LuckyLevel += x;
        ChangeStats?.Invoke();
    }

    public void UpdateMagnetLevel(int x) {
        MagnetLevel += x;
        ChangeStats?.Invoke();
    }

    public void UpdateDefenceLevel(int x) {
        DefenceLevel += x;
        ChangeStats?.Invoke();
    }

    public void UpdatePredictDistanceLevel(int x) {
        PredictDistanceLevel += x;
        ChangeStats?.Invoke();
    }
}

public interface IPlayerStatsReadOnly {
   int XMultiplierLevel { get; }
   int LuckyLevel { get; }
   int MagnetLevel { get; }
   int DefenceLevel { get; }
   int PredictDistanceLevel { get; }
    
    public event Action ChangeStats;
}

public interface IPlayerStatsWritable {
    int XMultiplierLevel { get; }
    int LuckyLevel { get; }
    int MagnetLevel { get; }
    int DefenceLevel { get; }
    int PredictDistanceLevel { get; }
    void UpdateXMultiplierLevel(int x = 1);
    void UpdateLuckyLevel(int x = 1);
    void UpdateMagnetLevel(int x = 1);
    void UpdateDefenceLevel(int x = 1);
    void UpdatePredictDistanceLevel(int x = 1);
}
