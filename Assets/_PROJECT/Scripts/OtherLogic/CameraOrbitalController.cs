    using Cysharp.Threading.Tasks.Triggers;
    using Unity.Cinemachine;
    using Unity.VisualScripting;
    using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraOrbitalController : MonoBehaviour {

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private CinemachineDecollider _cinemachineDecollider;
    [SerializeField] private float _sensitivity = 0.1f;
    
    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
    private Mouse _mouse;
    private bool _isOrbiting = false;

    private PlayerStateManager _playerStateManager;

    private bool _allowRotation = true;
    
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            // Вырубить
            _cinemachineDecollider.enabled = false;
            SetDefaultRotation();
            _allowRotation = false;
        }

        else if(state == PlayerState.Grounded || state == PlayerState.Cruisered){
            // _cinemachineDecollider.enabled = true;
            _allowRotation = true;
        }
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

    [SerializeField] private float _maxZoom;
    [SerializeField] private float _minZoom;
    [Range(0,1), SerializeField] private float _zoomSpeed;
    private void HandleZoom() {
        // Читаем колесико мыши
        float scroll = _mouse.scroll.ReadValue().y * _zoomSpeed; // Масштабируем
        
        if (Mathf.Abs(scroll) > 0.001f)
        {
            _orbitalFollow.RadialAxis.Value = Mathf.Clamp(
                _orbitalFollow.RadialAxis.Value - scroll,
                _minZoom, // минимальный зум
                _maxZoom    // максимальный зум
            );
        }
    }
}