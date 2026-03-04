using System;
using Architecture_M;
using MirraGames.SDK.Common;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraOrbitalController : MonoBehaviour {

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _sensitivity = 0.1f;
    [SerializeField] private float _joystickSensivity = 500f;
    [SerializeField] private Transform _walkPoint;
    [SerializeField] private Transform _flightPoint;
    
    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
    
    [SerializeField] private float _maxZoom;
    [SerializeField] private float _minZoom;
    [Range(0,1), SerializeField] private float _zoomSpeed;
    private Action _rotationHandler;
    [Header("Для теста пока через инспектор")]
    [SerializeField] private bool _isMobile;
    
    
    private Mouse _mouse;
    private bool _isOrbiting;


    private bool _allowRotation = true;
    private float _defaultX;
    private float _defaultY;
    
    public float DefaultFov => _isMobile ? _playerConfig.MobileFov : _playerConfig.DesktopFov;
    public float CurrentFovPercent => _orbitalFollow.RadialAxis.Value / _maxZoom;
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private SettingsManager _settings;
    [Inject] private PlayerConfig _playerConfig;
    
    // Если выбран десктоп ввод то не прокидывается сань помоги(((
    // [Inject] private IOrbitalRotationInput _orbitalRotationInput;
    // Сделал InjectOptional
    [InjectOptional] private IOrbitalRotationInput _orbitalRotationInput;
    [Inject] private IDeviceTypeProvider _deviceType;
    [Inject] private IInputActivity _inputActivity;

    
    
    private void OnEnable() {
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _settings.CameraValueChanged += SettingsOnCameraValueChanged;
    }


    
    private void Start() {
        SettingsOnCameraValueChanged(_settings.CameraZoomValue);
        
        // К релизу врубать
        // _isMobile = _deviceType.DeviceType == DeviceTypeEnum.Mobile;
        if (_isMobile)
            _rotationHandler = HandleJoystickOrbit;
        else {
            // Получаем ссылку на мышь
            _mouse = Mouse.current;
            _rotationHandler = HandleMouseOrbit;
        }
        _defaultX = _orbitalFollow.HorizontalAxis.Value;
        _defaultY = _orbitalFollow.VerticalAxis.Value;
    }

    private void SettingsOnCameraValueChanged(float percent) {
        float zoomValue = Mathf.Lerp(_minZoom, _maxZoom, percent);
        ChangeZoom(zoomValue);
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.TrampolineJumping) {
            return;
        }
        if (state == PlayerState.Flight) {
            // Вырубить
            SetDefaultRotation();
            _allowRotation = false;
            SetWalkPoint(false);
        }

        else if(state != PlayerState.Grounded && state != PlayerState.Cruisered){
            _allowRotation = true;
            SetWalkPoint(true);
        }
    }

    private void SetWalkPoint(bool setWalk) {
        if (setWalk) {
            _cinemachineCamera.Follow = _walkPoint;
            return;
        }
        _cinemachineCamera.Follow = _flightPoint;
    }


    private void SetDefaultRotation() {
        _orbitalFollow.HorizontalAxis.Value = _defaultX;
        _orbitalFollow.VerticalAxis.Value = _defaultY; 
    }
    
    private void Update() {
        _rotationHandler.Invoke();
    }

    private void HandleMouseOrbit() {
        // Проверяем нажатие правой кнопки мыши
        if (_mouse.rightButton.wasPressedThisFrame) {
            StartOrbiting();
        }
        else if (_mouse.rightButton.wasReleasedThisFrame) {
            StopOrbiting();
        }
        
        // Вращение
        if (_isOrbiting && _allowRotation) {
            OrbitCamera();
        }
        
        HandleZoom();
    }

    private void StartOrbiting() {
        _isOrbiting = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void StopOrbiting()
    {
        _isOrbiting = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    
    private void HandleJoystickOrbit() {
        if (!_allowRotation) return;

        Vector2 input = _orbitalRotationInput.OrbitalDirection;

        if (input.sqrMagnitude < 0.001f) 
            return;

        float joyX = input.x * _sensitivity * _joystickSensivity * Time.deltaTime;
        float joyY = input.y * _sensitivity * _joystickSensivity * Time.deltaTime;

        _orbitalFollow.HorizontalAxis.Value += joyX;
        _orbitalFollow.VerticalAxis.Value -= joyY;

        _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
            _orbitalFollow.VerticalAxis.Value,
            _orbitalFollow.VerticalAxis.Range.x,
            _orbitalFollow.VerticalAxis.Range.y
        );
    }
    
    
    private void OrbitCamera() {
        // Читаем дельту движения мыши
        Vector2 delta = _mouse.delta.ReadValue();
        
        // Применяем чувствительность
        float mouseX = delta.x * _sensitivity;
        float mouseY = delta.y * _sensitivity;
        
        // Вращаем камеру
        _orbitalFollow.HorizontalAxis.Value += mouseX;
        _orbitalFollow.VerticalAxis.Value -= mouseY; // Инвертируем Y
        
 
        // Ограничения
        _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
            _orbitalFollow.VerticalAxis.Value,
            _orbitalFollow.VerticalAxis.Range.x,
            _orbitalFollow.VerticalAxis.Range.y
        );
    }


    private void HandleZoom() {
        float scroll = _mouse.scroll.ReadValue().y * _zoomSpeed; // Масштабируем
        float zoomValue = _orbitalFollow.RadialAxis.Value - scroll;
        if (Mathf.Abs(scroll) > 0.001f) {
            ChangeZoom(zoomValue);
        }
    }

    private void ChangeZoom(float zoomValue) {
        _orbitalFollow.RadialAxis.Value = Mathf.Clamp(
            zoomValue,
            _minZoom, 
            _maxZoom
        );
        if (_settings.SettingsIsOpen) {
            _settings.ChangeCameraZoomSilent();
        }
    }
}