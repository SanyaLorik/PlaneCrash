using SanyaBeerExtension;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/CameraConfig")]
public class CameraConfig : ScriptableObject {
    [Header("Дефолтные значения в процентах")]
    [field: SerializeField, Range(0,1)] public float MobileCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DesktopCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float FlightCameraFov { get; private set; }
    [field: SerializeField, Range(0,1)] public float DefaultCameraSens { get; private set; }
    [field: SerializeField, Range(0,1)] public float ZoomSpeed { get; private set; }
    
    [Header("Множители сенсы")]
    [field: SerializeField] public float JoystickSensivityMultiplier  { get; private set; } = 2000f;
    [field: SerializeField] public float MouseSensivityMultiplier { get; private set; } = 0.1f;
    
    [Header("Ограничители")]
    [field: SerializeField] public PairedValue<float> ZoomDiapasone  { get; private set; }
    [field: SerializeField] public float MinSensValue  { get; private set; }
}