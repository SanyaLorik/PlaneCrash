using System;
using Architecture_M;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsManager : MonoBehaviour {
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private Button _settingsButtonOpen;
    [SerializeField] private Button _settingsButtonClose;
    
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Slider _cameraSlider;
    [SerializeField] private CameraOrbitalController _camera;

    
    private float _musicValue;
    private float _effectsValue;
    private float _cameraZoomValue;

    public float MusicValue => _musicValue;
    public float EffectsValue => _effectsValue;
    public float CameraZoomValue => _cameraZoomValue;
    
    
    
    private const string MusicKey = "settings_music";
    private const string EffectsKey = "settings_effects";
    private const string CameraKey = "settings_camera";
    
    
    public event Action<float> MusicValueChanged;
    public event Action<float> EffectsValueChanged;
    public event Action<float> CameraValueChanged;
    
    [Inject] private IInputActivity _inputActivity;
    
    
    private void Awake() {
        _settingsCanvas.DisactiveSelf();

        _musicValue = PlayerPrefs.GetFloat(MusicKey, 1f);
        _effectsValue = PlayerPrefs.GetFloat(EffectsKey, 1f);
        
        _cameraZoomValue = PlayerPrefs.GetFloat(CameraKey, _camera.DefaultFov);

        _musicSlider.SetValueWithoutNotify(_musicValue);
        _effectsSlider.SetValueWithoutNotify(_effectsValue);
        _cameraSlider.SetValueWithoutNotify(_cameraZoomValue);

        _settingsButtonOpen.AddListenerWithSound(OpenSettings);
        _settingsButtonClose.AddListenerWithSound(CloseSettings);

        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _effectsSlider.onValueChanged.AddListener(ChangeEffectsVolume);
        _cameraSlider.onValueChanged.AddListener(ChangeCameraZoom);
    }


    private void ChangeCameraZoom(float value) {
        _cameraZoomValue = value;
        PlayerPrefs.SetFloat(CameraKey, value);
        CameraValueChanged?.Invoke(value);
    }
    
    public void ChangeCameraZoomSilent() {
        PlayerPrefs.SetFloat(CameraKey, _camera.CurrentFovPercent);
        _cameraSlider.SetValueWithoutNotify(_camera.CurrentFovPercent);
        _cameraZoomValue = _camera.CurrentFovPercent;
    }

    private void ChangeEffectsVolume(float value) {
        _effectsValue = value;
        PlayerPrefs.SetFloat(EffectsKey, value);
        EffectsValueChanged?.Invoke(value);
    }

    
    public bool SettingsIsOpen => _settingsCanvas.activeSelf;
    private void ChangeMusicVolume(float value) {
        _musicValue = value;
        PlayerPrefs.SetFloat(MusicKey, value);
        MusicValueChanged?.Invoke(value);
    }

    private void OpenSettings() {
        _settingsCanvas.ActiveSelf();
        _inputActivity.Disable();
        // При открытии оно подгрузит с камеры измененный
        ChangeCameraZoomSilent();
    }




    private void CloseSettings() {
        _settingsCanvas.DisactiveSelf();
        _inputActivity.Enable();
        PlayerPrefs.Save();
    }
    
    
}