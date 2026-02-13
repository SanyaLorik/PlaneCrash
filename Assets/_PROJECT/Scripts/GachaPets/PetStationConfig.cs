using _PROJECT.Scripts.GachaPets;
using UnityEngine;

[CreateAssetMenu(fileName = "PetStationConfig", menuName = "Configs/PetStationConfig")]
public class PetStationConfig : ScriptableObject {
    [field: SerializeField] public PetChance[] Pets { get; private set; }
    [field: SerializeField] public GameObject Prefab  { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
}