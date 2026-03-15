using SanyaBeerExtension;
using UnityEngine;


[CreateAssetMenu(fileName = "BotsManagerConfig", menuName = "Configs/BotsManagerConfig")]
public class BotsManagerConfig : ScriptableObject {
    [field: SerializeField] public PairedValue<int> CountSpeakingBotsPerTime  { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToSpeak { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToChangeSkin { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToChangeNickname { get; private set; }
    [field: SerializeField] public PairedValue<int> PetCount { get; private set; }
    
    [Header("Movement")]
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanceToJump { get; private set; }
    [field: SerializeField, Range(0,1)] public float ChanseToGoPlayer { get; private set; }
    [field: SerializeField] public PairedValue<float> StoppingDistance { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToStayOnPoint { get; private set; }
    
    
   
}
