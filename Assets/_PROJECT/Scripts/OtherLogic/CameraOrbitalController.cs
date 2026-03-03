using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraOrbitalController : MonoBehaviour {

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _sensitivity = 0.1f;
    [SerializeField] private Transform _walkPoint;
    [SerializeField] private Transform _flightPoint;
    
    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
    
    [SerializeField] private float _maxZoom;
    [SerializeField] private float _minZoom;
    [Range(0,1), SerializeField] private float _zoomSpeed;
    
    private Mouse _mouse;
    private bool _isOrbiting;


    private bool _allowRotation = true;
    
    [Inject] private PlayerStateManager _playerStateManager;
    [Inject] private SettingsManager _settings;
    
    
    private void OnEnable() {
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _settings.CameraValueChanged += SettingsOnCameraValueChanged;
    }

    private void Start() {
        SettingsOnCameraValueChanged(_settings.CameraZoomValue);
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


    private float _defaultMouseX;
    private float _defaultMouseY;
    private void Awake() {
        // Получаем ссылку на мышь
        _mouse = Mouse.current;
        _defaultMouseX = _orbitalFollow.HorizontalAxis.Value;
        _defaultMouseY = _orbitalFollow.VerticalAxis.Value;
    }


    private void SetDefaultRotation() {
        _orbitalFollow.HorizontalAxis.Value = _defaultMouseX;
        _orbitalFollow.VerticalAxis.Value = _defaultMouseY; 
    }
    
    private void Update() {
        if (_orbitalFollow == null || _mouse == null) return;
        
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
        
        // Зум колесиком (всегда работает)
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
    }
}