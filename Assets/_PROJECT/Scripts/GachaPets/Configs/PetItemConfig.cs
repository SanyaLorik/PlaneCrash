using System;
using UnityEngine;

[Serializable]
public enum PetStatus {
    Default,
    Rare,
    Legendary,
} 


[CreateAssetMenu(fileName = "PetItemConfig", menuName = "Configs/PetItemConfig")]
public class PetItemConfig : ScriptableObject {
    [field: SerializeField] public float Modifier  { get; private set; }
    [field: SerializeField] public GameObject Prefab  { get; private set; }
    [field: SerializeField] public Sprite Sprite  { get; private set; }
    [field: SerializeField] public string Id  { get; private set; }
    [field: SerializeField] public PetStatus PetStatus  { get; private set; }
    [field: SerializeField] public long PriceIfBought  { get; private set; }
}