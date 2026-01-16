using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : MonoBehaviour {
    private PlayerConfig _config;

    private AnimationCurve _currentCurve;
    private float _segmentDuration;
    private float _expandedTime = 0;
    private Vector3 _initialPos;
    private Vector3 _targetPos;
    private bool _isBusted;
    
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private float _currentRoll;

    private CancellationTokenSource _playerCTS;
    private PlayerStateManager _stateManager;

    public float PlayerSpeed => _config.SpeedForce;
    public event Action OnPlayerFlight; 
    
    [Inject]
    public void Init(PlayerConfig config, PlayerStateManager stateManager) {
        _stateManager =  stateManager;
        _config = config;
        
        _stateManager.ChangeState += OnChangeSpaceRotation;
    }
    
    
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
    }
    
    private void Start() {
        _playerCTS = new CancellationTokenSource();
        OnChangeSpaceRotation(PlayerState.Walking);
        TpPlayerInSpawn();
    }

    private void FixedUpdate() {
        if (_stateManager.CurrentState == PlayerState.Walking) {
            Walk();
        }
        else if(_stateManager.CurrentState == PlayerState.Flight) {
            FlightLogic();
            VisualRotate();
        }
    }

    
    public void TpPlayerInSpawn() {
        transform.position = _config.PlayerSpawnPosition;
    }


    private void OnChangeSpaceRotation(PlayerState playerState) {
        if (playerState == PlayerState.Flight) {
            PlayerRotateLocalX(-25, playerState);
            _rb.useGravity = false;
        }
        else {
            PlayerRotateLocalX(-80, playerState);
            _rb.useGravity = true;
        }
    }

    private async UniTask PlayerRotateLocalX(float _targetPosAngleX, PlayerState playerState) {
        float duration = 1f;
    
        Vector3 currentLocalEuler = transform.localEulerAngles;
        Vector3 _targetPosLocalEuler;
        if (playerState == PlayerState.Walking) {
            _targetPosLocalEuler = new Vector3(_targetPosAngleX, currentLocalEuler.y, currentLocalEuler.z);
        }
        else {
            _targetPosLocalEuler = new Vector3(_targetPosAngleX, 0f, 0f);
        }
    
        Quaternion startRot = transform.localRotation;
        Quaternion _targetPosRot = Quaternion.Euler(_targetPosLocalEuler);
    
        float elapsedTime = 0;
    
        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
        
            transform.localRotation = Quaternion.Slerp(startRot, _targetPosRot, t);
        
            await UniTask.Yield(_playerCTS.Token);
        }
    
        transform.localRotation = _targetPosRot;
    }

    public void OnMove(InputAction.CallbackContext context) {
        _moveInput = context.ReadValue<Vector2>();
    }
    

    private bool _secondJumpAllowed = true;
    public void OnJump(InputAction.CallbackContext context) {
        if (!context.performed || _stateManager.CurrentState != PlayerState.Walking) return;     // реагируем только на нажатие
        
        Vector3 origin = transform.position;
        
        if (Physics.Raycast(origin, Vector3.down,  0.1f, _config.FloorMask)) {
            _rb.AddForce(Vector3.up * _config.JumpForce, ForceMode.Impulse);
            _secondJumpAllowed = true;
        }
        else if (_secondJumpAllowed) {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * _config.SecondJumpForce, ForceMode.Impulse);
            _secondJumpAllowed = false;
        }
    }

    private void Walk() {
        // Сильнее гравитация работает
        _rb.AddForce(Physics.gravity * (_config.GravityScale - 1) * _rb.mass);
        
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight   = cam.right;

        // убираем вертикаль
        camForward.y = 0f;
        camRight.y   = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move =
            camRight   * _moveInput.x +
            camForward * _moveInput.y;

        
        Vector3 moveStep = move * _config.WalkSpeed * Time.fixedDeltaTime;
        if (Physics.Raycast(
                _rb.position,
                moveStep.normalized,
                out RaycastHit hit,
                moveStep.magnitude + _config.WallOffset,
                _config.FloorMask)) {
            moveStep = moveStep.normalized * (hit.distance - _config.WallOffset);
        }
        
        _rb.MovePosition(_rb.position + moveStep);
        WalkRotate(move);

    }

    private void WalkRotate(Vector3 move) {
        if (move.sqrMagnitude > 0.0001f) {
            float _targetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                _targetPosY,
                _config.RotateSpeed * Time.fixedDeltaTime
            );
            // Крутится только Y
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x, 
                y,                        
                transform.eulerAngles.z  
            );
        }
    }


    private void FlightLogic() {
        Vector3 newPos =  transform.position;
        newPos.x += _moveInput.x * _config.RotateSpeed * Time.fixedDeltaTime;
        
        newPos.x = Mathf.Clamp(newPos.x, _config.XMovement.From, _config.XMovement.To);
        if (!_isBusted) {
            newPos.z += _config.SpeedForce * Time.fixedDeltaTime;
            newPos.y -= _config.FallingSpeed * Time.fixedDeltaTime;
        }
        else {
            float normalizedTime = _expandedTime / _segmentDuration;
            
            float height = _currentCurve.Evaluate(normalizedTime) * _config.JumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(_initialPos.y, _targetPos.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(_initialPos.z, _targetPos.z, normalizedTime);
            _expandedTime += Time.fixedDeltaTime;
            if (_expandedTime >= _segmentDuration) {
                _isBusted = false;
            }
        }
        transform.position = newPos;
    }


    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        _currentCurve = curve;
        _expandedTime = 0f;
        _isBusted = true;
        _initialPos = transform.position;
        _targetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, _targetPos);
        _segmentDuration = distance / _config.SpeedForce; 
    } 

    
  
    private void VisualRotate() {
        float _targetPosRoll = -_moveInput.x * _config.MaxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, _targetPosRoll, Time.fixedDeltaTime * _config.RotateSpeed);

        Vector3 euler = transform.localEulerAngles;
        euler.z = _currentRoll;
        transform.localEulerAngles = euler;
    }
    
    
    private void OnDestroy() {
        _playerCTS?.Cancel();
        _playerCTS?.Dispose();
    }
    
}
