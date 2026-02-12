using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RangData {
    public int Id;
    public int Money;
    public Sprite Sprite;
}


[CreateAssetMenu(fileName = "RangConfig", menuName = "Configs/RangConfig")]
public class RangConfig : ScriptableObject {
    [field: SerializeField] public List<RangData> Rangs { get; private set; }
}
