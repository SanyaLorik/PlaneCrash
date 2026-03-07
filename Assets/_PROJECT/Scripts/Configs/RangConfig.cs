using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangData {
    public int Id;
    public long Money;
    public long RewardMoney;
    public Sprite Sprite;
}


[CreateAssetMenu(fileName = "RangConfig", menuName = "Configs/RangConfig")]
public class RangConfig : ScriptableObject {
    [field: SerializeField] public List<RangData> Rangs { get; private set; }
}
