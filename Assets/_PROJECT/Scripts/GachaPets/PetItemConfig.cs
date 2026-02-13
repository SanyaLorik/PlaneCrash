using UnityEngine;


[CreateAssetMenu(fileName = "PetItemConfig", menuName = "Configs/PetItemConfig")]
public class PetItemConfig : ScriptableObject {
    [field: SerializeField] public float Modifier  { get; private set; }
    [field: SerializeField] public GameObject Prefab  { get; private set; }
    [field: SerializeField] public Sprite Sprite  { get; private set; }
}