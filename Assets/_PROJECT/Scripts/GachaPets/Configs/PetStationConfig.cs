using UnityEngine;

[CreateAssetMenu(fileName = "PetStationConfig", menuName = "Configs/PetStationConfig")]
public class PetStationConfig : ScriptableObject {
    [field: SerializeField] public PetChance[] Pets { get; private set; }
    [field: SerializeField] public long Price { get; private set; }
    [field: SerializeField] public Sprite EggIcon { get; private set; }
}   