using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Zenject;

public class PlayerMovement : FlightObject {
    private PlayerConfig _config;

    private bool _isBusted;


    public Rigidbody Rb { get; private set; } 
    private Vector2 _moveInput;
    private float _currentRoll;

    private PlayerStateManager _stateManager;
    private LevelBounds _levelBounds;
    private IPlayerStatsReadOnly _playerStats;

    public float PlayerSpeed => _config.SpeedForce;
    public event Action SetBoost;
    
    [Inject]
    public void Init(PlayerConfig config, PlayerStateManager stateManager, LevelBounds levelBounds, IPlayerStatsReadOnly playerStats) {
        _stateManager =  stateManager;
        _config = config;
        _levelBounds = levelBounds;
        
        _stateManager.ChangeState += OnChangeSpaceRotation;

        _playerStats = playerStats;
    }
    
    
    
    
    
    private void Awake() {
        Rb = GetComponent<Rigidbody>();
        Rb.useGravity = true;
    }
    
    private void Start() {
        OnChangeSpaceRotation(PlayerState.Walking);
        TpPlayerInSpawn();
    }

    
    private void Update() {
        if(_stateManager.CurrentState == PlayerState.Flight) {
            VisualRotate();
        }
    }
    

    [SerializeField] private int LifesCount;
    private void FixedUpdate() {
        if (_stateManager.CurrentState == PlayerState.Walking) {
            Walk();
        }
        else if(_stateManager.CurrentState == PlayerState.Flight) {
            FlightLogic();
        }
    }
    

    
    public void TpPlayerInSpawn() {
        transform.position = _config.PlayerSpawnPosition;
        Rb.linearVelocity = Vector3.zero;
    }




    public bool TryToKill() => LifesCount-- <= 0;
    
    private void ResetLifes() => LifesCount = _playerStats.DefenceCount;
    


    public bool IsBombed;
    public void SetPlayerIsBombed() {
        _isBusted = false;
        IsBombed = true;
        // Rb.useGravity = true;
    }


    private void OnChangeSpaceRotation(PlayerState playerState) {
        _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
        if (playerState == PlayerState.Flight) {
            ResetLifes();
            IsBombed = false;
            RotateLocalXAsync(-25, playerState, _token).Forget();
            Rb.useGravity = false;
        }
        else {
            RotateLocalXAsync(-80, playerState, _token).Forget();
            Rb.useGravity = true;
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
            Rb.AddForce(Vector3.up * _config.JumpForce, ForceMode.Impulse);
            _secondJumpAllowed = true;
        }
        else if (_secondJumpAllowed) {
            Rb.linearVelocity = new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);
            Rb.AddForce(Vector3.up * _config.SecondJumpForce, ForceMode.Impulse);
            _secondJumpAllowed = false;
        }
    }

    private void Walk() {
        // усиленная гравитация
        Rb.AddForce(Physics.gravity * (_config.GravityScale - 1) * Rb.mass);

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
        Vector3 lowOrigin  = Rb.position + Vector3.up * 0.05f;
        Vector3 highOrigin = Rb.position + Vector3.up * _config.StepHeight;

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
            Rb.MovePosition(Rb.position + stepUp + moveStep);
        }
        else if (!hitLow) {
            // обычное движение
            Rb.MovePosition(Rb.position + moveStep);
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
            newPos.y -= _config.FallingSpeed * 25f * Time.fixedDeltaTime;
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


    [SerializeField] private float _getObjectsCooldownSeconds;
    public bool ObjectGetAllow { get; private set; } = true;

    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        if (!ObjectGetAllow) return;

        _currentCurve = curve;
        _expandedTime = 0f;
        _isBusted = true;
        _initialPos = transform.position;
        TargetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, TargetPos);
        _segmentDuration = distance / _config.SpeedForce; 
        SetBoost?.Invoke();
        ObjectGetAllow = false;
        StartCoroutine(ObjectAllowCooldown());
    }

    private IEnumerator ObjectAllowCooldown() {
        yield return new WaitForSeconds(_getObjectsCooldownSeconds);
        ObjectGetAllow = true;
    } 

    
  
    private float _rollVelocity;
    [SerializeField] private float _smoothTime = 0.3f;

    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _config.MaxRotate;

        _currentRoll = Mathf.SmoothDamp(
            _currentRoll,
            targetRoll,
            ref _rollVelocity,
            _smoothTime // время сглаживания
        );

        Vector3 euler = transform.localEulerAngles;
        euler.z = _currentRoll;
        transform.localEulerAngles = euler;
    }




    
}
