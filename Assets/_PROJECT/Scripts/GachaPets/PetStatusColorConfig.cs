using System;
using UnityEngine;


[Serializable]
public struct PetColor {
    public PetStatus Status;
    public Color Color;
}


[CreateAssetMenu(fileName = "PetStatusColor", menuName = "Configs/PetStatusColor")]
public class PetStatusColorConfig : ScriptableObject {
    [field: SerializeField] public PetColor[] PetColor { get; private set; }


    public Color GetColorByStatus(PetStatus status) {
        foreach (var petColor in PetColor) {
            if (petColor.Status == status) {
                return petColor.Color;
            } 
        }

        return Color.navajoWhite;
    }
}

