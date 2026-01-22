using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : FlightObject {
    private PlayerConfig _config;

    private bool _isBusted;

    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private float _currentRoll;

    private PlayerStateManager _stateManager;
    private LevelBounds _levelBounds;

    public float PlayerSpeed => _config.SpeedForce;
    public event Action SetBoost;
    
    [Inject]
    public void Init(PlayerConfig config, PlayerStateManager stateManager, LevelBounds levelBounds) {
        _stateManager =  stateManager;
        _config = config;
        _levelBounds = levelBounds;
        
        _stateManager.ChangeState += OnChangeSpaceRotation;
    }
    
    
    
    
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
    }
    
    private void Start() {
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


    public bool IsBombed;
    public void SetPlayerIsBombed() {
        _isBusted = false;
        IsBombed = true;
        // _rb.useGravity = true;
    }


    private void OnChangeSpaceRotation(PlayerState playerState) {
        _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
        if (playerState == PlayerState.Flight) {
            IsBombed = false;
            RotateLocalXAsync(-25, playerState, _token).Forget();
            _rb.useGravity = false;
        }
        else {
            RotateLocalXAsync(-80, playerState, _token).Forget();
            _rb.useGravity = true;
        }
    }

    private async UniTask RotateLocalXAsync(float TargetPosAngleX, PlayerState playerState, CancellationToken token) {
        float duration = 1f;
    
        Vector3 currentLocalEuler = transform.localEulerAngles;
        Vector3 TargetPosLocalEuler;
        if (playerState == PlayerState.Walking) {
            TargetPosLocalEuler = new Vector3(TargetPosAngleX, currentLocalEuler.y, currentLocalEuler.z);
        }
        else {
            TargetPosLocalEuler = new Vector3(TargetPosAngleX, 0f, 0f);
        }
    
        Quaternion startRot = transform.localRotation;
        Quaternion TargetPosRot = Quaternion.Euler(TargetPosLocalEuler);
    
        float elapsedTime = 0;
    
        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
        
            transform.localRotation = Quaternion.Slerp(startRot, TargetPosRot, t);
        
            await UniTask.Yield(token);
        }
    
        transform.localRotation = TargetPosRot;
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
        // усиленная гравитация
        _rb.AddForce(Physics.gravity * (_config.GravityScale - 1) * _rb.mass);

        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight   = cam.right;

        camForward.y = 0f;
        camRight.y   = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move =
            camRight   * _moveInput.x +
            camForward * _moveInput.y;

        if (move.sqrMagnitude < 0.001f)
            return;

        Vector3 moveDir  = move.normalized;
        Vector3 moveStep = moveDir * _config.WalkSpeed * Time.fixedDeltaTime;

        float checkDist = moveStep.magnitude + _config.WallOffset;

        // === STEP LOGIC ===
        Vector3 lowOrigin  = _rb.position + Vector3.up * 0.05f;
        Vector3 highOrigin = _rb.position + Vector3.up * _config.StepHeight;

        bool hitLow = Physics.Raycast(
            lowOrigin,
            moveDir,
            out RaycastHit lowHit,
            checkDist,
            _config.FloorMask
        );

        bool hitHigh = Physics.Raycast(
            highOrigin,
            moveDir,
            checkDist,
            _config.FloorMask
        );

        if (hitLow && !hitHigh) {
            // это ступенька — шагаем вверх
            Vector3 stepUp = Vector3.up * _config.StepHeight;
            _rb.MovePosition(_rb.position + stepUp + moveStep);
        }
        else if (!hitLow) {
            // обычное движение
            _rb.MovePosition(_rb.position + moveStep);
        }
        // если hitLow && hitHigh - это стена, никуда не идём
        WalkRotate(move);
    }


    private void WalkRotate(Vector3 move) {
        if (move.sqrMagnitude > 0.0001f) {
            float TargetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                TargetPosY,
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
        if (IsBombed) {
            newPos.y -= _config.FallingSpeed * 10 * Time.fixedDeltaTime;
            transform.position = newPos;
            return;
        }
        
        newPos.x += _moveInput.x * _config.RotateSpeed * Time.fixedDeltaTime;
        
        newPos.x = Mathf.Clamp(newPos.x, _levelBounds.LeftX, _levelBounds.RightX);
        if (!_isBusted) {
            newPos.z += _config.SpeedForce * Time.fixedDeltaTime;
            newPos.y -= _config.FallingSpeed * Time.fixedDeltaTime;
        }
        else {
            float normalizedTime = _expandedTime / _segmentDuration;
            
            float height = _currentCurve.Evaluate(normalizedTime) * _config.JumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, normalizedTime);
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
        TargetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, TargetPos);
        _segmentDuration = distance / _config.SpeedForce; 
        SetBoost?.Invoke();
    } 

    
  
    private void VisualRotate() {
        float TargetPosRoll = -_moveInput.x * _config.MaxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, TargetPosRoll, Time.fixedDeltaTime * _config.RotateSpeed);

        Vector3 euler = transform.localEulerAngles;
        euler.z = _currentRoll;
        transform.localEulerAngles = euler;
    }
    

    
}
