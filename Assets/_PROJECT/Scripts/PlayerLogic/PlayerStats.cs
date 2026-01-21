using System;
using UnityEngine;

public class PlayerStats : IPlayerStatsReadOnly, IPlayerStatsWritable {
    public float XMultiplier { get; private set; } = 1f;
    public event Action ChangeStats;

    public void MultiplyXMultiplier(float x) {
        XMultiplier *= Mathf.Max(1f, x);
        ChangeStats?.Invoke();
    }

}

public interface IPlayerStatsReadOnly {
    float XMultiplier { get; }
    public event Action ChangeStats;
}

public interface IPlayerStatsWritable {
    float XMultiplier { get; }
    void MultiplyXMultiplier(float x);
}
