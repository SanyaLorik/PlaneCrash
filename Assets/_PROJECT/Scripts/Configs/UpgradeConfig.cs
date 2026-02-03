using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;


[Serializable]
public struct UpgradeInfo {
    public float K;
    public float BaseValue;
    public float StartPrice;
    public float PriceMultiplier;
}


[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Configs/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject {
    public UpgradeInfo XMultiplierUpgrade;
    public UpgradeInfo LuckyUpgrade;
    public UpgradeInfo MagneteUpgrade;
    public UpgradeInfo DefenceUpgrade;
    public UpgradeInfo PredictionUpgrade;


    public float MagneteSizeGrowSpeed;
}
