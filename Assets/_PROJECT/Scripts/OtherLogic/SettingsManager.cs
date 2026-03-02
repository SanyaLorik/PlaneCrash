using System;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private Button _settingsButtonOpen;
    [SerializeField] private Button _settingsButtonClose;
    
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Slider _cameraSlider;

    
    public event Action<float> MusicValueChanged;
    public event Action<float> EffectsValueChanged;
    public event Action<float> CameraValueChanged;
    
    
    private void Awake() {
        _settingsCanvas.DisactiveSelf();
        // Кнопки
        _settingsButtonOpen.AddListenerWithSound(OpenSettings);
        _settingsButtonClose.AddListenerWithSound(CloseSettings);
        // Слайдеры
        _musicSlider.onValueChanged.AddListener((value) => MusicValueChanged?.Invoke(value));
        _effectsSlider.onValueChanged.AddListener((value) => EffectsValueChanged?.Invoke(value));
        _cameraSlider.onValueChanged.AddListener((value) => CameraValueChanged?.Invoke(value));
        
        
    }

    private void OpenSettings() {
        _settingsCanvas.ActiveSelf();
    }
    
    
    private void CloseSettings() {
        _settingsCanvas.DisactiveSelf();
    }
    
    
}
