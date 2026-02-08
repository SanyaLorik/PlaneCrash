using System;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;

[Serializable]
public class GameSavePC : GameSaveBase {
    public long Money = 0;
    public List<UpgradeData> Upgrades = new ();
    public bool TutorialPassed = false;
}


[Serializable]
public class UpgradeData {
    public int Level = 1;
    public int ID = 0;
}