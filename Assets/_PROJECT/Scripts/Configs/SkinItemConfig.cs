using UnityEngine;


[CreateAssetMenu(fileName = "SkinItem", menuName = "Configs/SkinItem")]
public class SkinItemConfig : ScriptableObject {
    [field: SerializeField] public string Id { get; private set; } 
    [field: SerializeField] public long Price { get; private set; } 
    [field: SerializeField] public GameObject SkinPrefab { get; private set; }
    [field: SerializeField] public Avatar Avatar { get; private set; } 

}
