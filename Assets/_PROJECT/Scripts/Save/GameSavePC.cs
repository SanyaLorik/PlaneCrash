using System;
using System.Collections.Generic;
using Architecture_M;
using UnityEngine;

[Serializable]
public class GameSavePC : GameSaveBase {
    public long Money = 0;
    public List<Upgrades> Upgrades = new ();
}


[Serializable]
public class Upgrades {
    public int Level = 0;
    public int ID = 0;
}