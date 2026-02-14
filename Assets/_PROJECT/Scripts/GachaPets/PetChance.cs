using System;
using UnityEngine;

[Serializable]
public struct PetChance {
    [field: SerializeField] public PetItemConfig PetItemConfig  { get; private set; }
    [field: SerializeField][field: Range(0,1)] public float Chance { get; private set; }
}
