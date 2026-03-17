using System;
using UnityEngine;


[Serializable]
public struct UpgradeInfo {
    public UpgradeType UpgradeType;
    public string Id;
    public float BaseValue;
    public float StartPrice;
}

[Serializable]
public enum UpgradeType {
    Multiplier,
    Magnet,
    Lucky,
    Predict,
    Defence
}


[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Configs/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject {
    public UpgradeInfo XMultiplierUpgrade;
    public UpgradeInfo LuckyUpgrade;
    public UpgradeInfo MagneteUpgrade;
    public UpgradeInfo DefenceUpgrade;
    public UpgradeInfo PredictionUpgrade;
    public float MagneteSizeGrowSpeed;
    [Header("Множитель магнита с уровнем")]
    public float MagnetLevelK;
}
