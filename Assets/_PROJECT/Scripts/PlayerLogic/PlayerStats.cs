using System;
using UnityEngine;

public class PlayerStats : IPlayerStatsReadOnly, IPlayerStatsWritable {
    public float XMultiplier { get; private set; } = 1f;
    public float LuckyMultiplier { get; private set; } = 1f;
    public float MagnetSpeed { get; private set; } = 0;
    public int DefenceCount { get; private set; }
    public event Action ChangeStats;

    public void UpdateXMultiplier(float x) {
        XMultiplier *= Mathf.Max(1f, x);
        ChangeStats?.Invoke();
    }

    public void UpdateLucky(float x) {
        LuckyMultiplier *= Mathf.Max(1f, x);
        ChangeStats?.Invoke();
    }

    public void UpdateMagnet(float x) {
        MagnetSpeed += x;
        ChangeStats?.Invoke();
    }

    public void UpdateDefence(int x) {
        DefenceCount += x;
        ChangeStats?.Invoke();
    }
}

public interface IPlayerStatsReadOnly {
    float XMultiplier { get; }
    float LuckyMultiplier { get; }
    float MagnetSpeed { get; }
    int DefenceCount { get; }
    
    public event Action ChangeStats;
}

public interface IPlayerStatsWritable {
    float XMultiplier { get; }
    float LuckyMultiplier { get; }
    float MagnetSpeed { get; }
    int DefenceCount { get; }
    void UpdateXMultiplier(float x);
    void UpdateLucky(float x);
    void UpdateMagnet(float x);
    void UpdateDefence(int x);
}
