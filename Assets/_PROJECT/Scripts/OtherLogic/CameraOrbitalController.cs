using System;
using System.Collections;
using Architecture_M;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraOrbitalController : MonoBehaviour {
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _walkPoint;
    [SerializeField] private Transform _flightPoint;

    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
    [SerializeField] private float _cameraSaveDelay = 1f;

    private Action _rotationHandler;

    private bool IsMobile => _deviceType.DeviceType == DeviceTypeEnum.Mobile;


    private Mouse _mouse;
    private bool _isOrbiting;


    private bool _allowRotation = true;
    private bool _allowZoom = true;
    private float _defaultX;
    private float _defaultY;

    public float DefaultFov => IsMobile ? _cameraConfig.MobileCameraFov : _cameraConfig.DesktopCameraFov;
    public float CurrentFovPercent => (_orbitalFollow.RadialAxis.Value - _cameraConfig.ZoomDiapasone.From) 
                                      / 
                                      (_cameraConfig.ZoomDiapasone.To - _cameraConfig.ZoomDiapasone.From);
    public float DefaultSens => _cameraConfig.DefaultCameraSens;

    private float Sensitivity => Mathf.Clamp(_settings.CameraSensValue, _cameraConfig.MinSensValue, 1f); 
    private float _walkZoomBeforFly;
    
    
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private SettingsManager _settings;
    [Inject] private PlayerConfig _playerConfig;
    [Inject] private CameraConfig _cameraConfig;

    // Если выбран десктоп ввод то не прокидывается сань помоги(((
    // [Inject] private IOrbitalRotationInput _orbitalRotationInput;
    // Сделал InjectOptional
    [InjectOptional] private IOrbitalRotationInput _orbitalRotationInput;
    [Inject] private IDeviceTypeProvider _deviceType;
    [Inject] private IInputActivity _inputActivity;


    private void OnEnable() {
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _settings.CameraZoomChanged += ChangeCameraZoomPercent;
        SystemEvents.WindowOpened += ForbidRotate;
        SystemEvents.ForbidZoomChanged += ForbidZoom;
    }


    private void Start() {
        ChangeCameraZoomPercent(_settings.CameraZoomValue);
        _walkZoomBeforFly = CurrentFovPercent;
        // К релизу врубать
        if (IsMobile)
            _rotationHandler = HandleJoystickOrbit;
        else {
            // Получаем ссылку на мышь
            _mouse = Mouse.current;
            _rotationHandler = HandleMouseOrbit;
        }
        _defaultX = _orbitalFollow.HorizontalAxis.Value;
        _defaultY = _orbitalFollow.VerticalAxis.Value;
        SetWalkPoint(true);
    }

    private void ChangeCameraZoomPercent(float percent) {
        float zoomValue = Mathf.Lerp(_cameraConfig.ZoomDiapasone.From, _cameraConfig.ZoomDiapasone.To, percent);
        ChangeZoom(zoomValue);
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.TrampolineJumping) {
            return;
        }
        if (state == PlayerState.Flight) {
            _walkZoomBeforFly = CurrentFovPercent;
            Debug.Log("Игрок полетел зум камеры: " + _walkZoomBeforFly);
            // Вырубить
            ChangeCameraZoomPercent(_cameraConfig.FlightCameraFov);
            SetDefaultRotation();
            _allowRotation = false;
            SetWalkPoint(false);
        }

        else if(state != PlayerState.Grounded && state != PlayerState.Cruisered){
            _allowRotation = true;
            SetWalkPoint(true);
            Debug.Log("Игрок вернулся на спавн: " + _walkZoomBeforFly);
            ChangeCameraZoomPercent(_walkZoomBeforFly);
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

        float joyX = input.x * Sensitivity * _cameraConfig.JoystickSensivityMultiplier * Time.deltaTime;
        float joyY = input.y * Sensitivity * _cameraConfig.JoystickSensivityMultiplier * Time.deltaTime;

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
        float mouseX = delta.x * Sensitivity * _cameraConfig.MouseSensivityMultiplier;
        float mouseY = delta.y * Sensitivity * _cameraConfig.MouseSensivityMultiplier;
        
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
        float scroll = _mouse.scroll.ReadValue().y * _cameraConfig.ZoomSpeed;
        float zoomValue = _orbitalFollow.RadialAxis.Value - scroll;
        if (Mathf.Abs(scroll) > 0.001f) {
            ChangeZoom(zoomValue);
        }
    }

    
    private void ChangeZoom(float zoomValue) {
        if(!_allowZoom) return;
        _orbitalFollow.RadialAxis.Value = Mathf.Clamp(
            zoomValue,
            _cameraConfig.ZoomDiapasone.From, 
            _cameraConfig.ZoomDiapasone.To
        );
        if (_settings.SettingsIsOpen) {
            _settings.ChangeCameraZoomSilent();
        }
        else {
            if (_waitCameraSave != null) {
                StopCoroutine(_waitCameraSave);
            }   
            _waitCameraSave = StartCoroutine(WaitCameraSave());
        }
    }
    private Coroutine _waitCameraSave;
    private IEnumerator WaitCameraSave() {
        yield return new WaitForSeconds(_cameraSaveDelay);
        _settings.ChangeCameraZoomSilent();
    }
    
    
    private void ForbidZoom(bool forbid) {
        _allowZoom = !forbid;
    }

    private void ForbidRotate(bool windowIsOpen) {
        _allowRotation = !windowIsOpen;
    }
}