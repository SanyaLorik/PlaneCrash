using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    [SerializeField] private List<UpgradeBase> _visualUpgrades;

    private void Awake() {
        foreach (var upgrade in _visualUpgrades) {
            upgrade.LoadLevel();
        }
        Debug.Log("LoadLevel");
    }

    public void AddNewUpgrade(UpgradeType upgradeType, int countNewLevels) {
        Debug.Log("AddNewUpgrade");
        var item = _visualUpgrades.Find(u => u.UpgradeInfo.UpgradeType == upgradeType);
        item.ApplyUpgrade(countNewLevels, false);
    }
}
