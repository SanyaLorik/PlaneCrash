using SanyaBeerExtension;
using UnityEngine;


[CreateAssetMenu(fileName = "BotsManagerConfig", menuName = "Configs/BotsManagerConfig")]
public class BotsManagerConfig : ScriptableObject {
    [field: SerializeField] public PairedValue<int> CountSpeakingBotsPerTime  { get; private set; }
    [field: SerializeField] public PairedValue<float> TimeToSpeak { get; private set; }
   
}
