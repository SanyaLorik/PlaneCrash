using System;
using System.Collections;
using System.Threading;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerMovement : FlightObject {
    [SerializeField] private float _smoothTime = 0.3f;
    [SerializeField] private int _currentLifesCount;
    [SerializeField] private JumpParticlesController _jumpParticlesController;


    
    public Rigidbody Rb { get; private set; } 
    public Transform Transform  => transform;


    public Vector2 MoveInput => _inputDirection2.Direction2;
    private float _currentRoll;
    private float _rollVelocity;
    public bool IsBusted {get; private set; }

    
    public bool IsBombed;

    public float PlayerSpeed => _config.SpeedForce;
    public float JumpHeight => _config.JumpHeight;
    public event Action SetBoost;
    public event Action OnJumpPressed;
    public event Action OnDoubleJumpPressed;
    public event Action<bool> OnRunningStateChanged;
    public event Action Floored;


    public bool IsRunning { get; private set; }

    [Inject] private PlayerConfig _config;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private LevelBounds _levelBounds;
    [Inject] private PlayerVisual _visual;
    [Inject] private IInputDirection2 _inputDirection2;
    [Inject] private IInputJumping _inputJumping;
    

    private void OnEnable() {
        _stateManager.ChangeState += OnChangeSpaceRotation;
        _inputJumping.OnJumped += OnJump;
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
    

    private void FixedUpdate() {
        if (_stateManager.CurrentState == PlayerState.Walking ||
            _stateManager.CurrentState == PlayerState.TrampolineJumping) {
            Walk();
        }
        else if(_stateManager.CurrentState == PlayerState.Flight) {
            FlightLogic();
        }
    }
    

    
    public void TpPlayerInSpawn() {
        _stateManager.ChangePlayerState(PlayerState.Walking);
        transform.position = _levelBounds.PlayerSpawnPoint.position;
        Rb.linearVelocity = Vector3.zero;
        _visual.TeleportParticles();
    }
    
    public void TpPlayerInBetZone() {
        transform.position = _levelBounds.BetZonePosition.position;
        Rb.linearVelocity = Vector3.zero;
        _visual.TeleportParticles();
    }




    public bool TryToKill() {
        _currentLifesCount--;
        _visual.StartDizzy();
        if (_currentLifesCount <= 0) {
            SetPlayerIsBombed();
        }
        return _currentLifesCount <= 0;
    } 
    
    private void ResetLifes() => _currentLifesCount = _upgradesCalculator.GetDefenceByLevel();
    


    private void SetPlayerIsBombed() {
        IsBusted = false;
        IsBombed = true;
    }


    private void OnChangeSpaceRotation(PlayerState playerState) {
        _tokenSource = new CancellationTokenSource();
        if (playerState == PlayerState.Flight) {
            ResetLifes();
            IsBombed = false;
            RotateLocalXAsync(-25, playerState, _tokenSource.Token).Forget();
            Rb.useGravity = false;
        }
        else if(playerState == PlayerState.Grounded || playerState == PlayerState.Cruisered){
            RotateLocalXAsync(-80, playerState, _tokenSource.Token).Forget();
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
    
    public void OnJump() {
        if (_stateManager.CurrentState != PlayerState.Walking) return;     // реагируем только на нажатие
        
        
        if (_jumpsUsed == 0) {
            Rb.linearVelocity = new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);
            DoJump(_config.JumpForce);
            _jumpParticlesController.Play();
            OnJumpPressed?.Invoke();
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1) {
            Rb.linearVelocity = new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);
            DoJump(_config.SecondJumpForce);
            OnDoubleJumpPressed?.Invoke();
            _jumpParticlesController.Play();
            _jumpsUsed = 2;
        }
    }

    private void DoJump(float force) {
        Rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    private int _jumpsUsed;
    private void OnCollisionEnter(Collision collision) {
        foreach (var contact in collision.contacts) {
            if (contact.normal.y > 0.5f) {
                _jumpsUsed = 0;
                Floored?.Invoke();
                break;
            }
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
            camRight   * MoveInput.x +
            camForward * MoveInput.y;

        bool hasInput = move.sqrMagnitude > 0.001f;
        
        if (hasInput != IsRunning) {
            IsRunning = hasInput;
            OnRunningStateChanged?.Invoke(IsRunning);
        }
        
        if (!hasInput)
            return;

        Vector3 moveDir  = move.normalized;
        Vector3 moveStep = moveDir * _config.WalkSpeed * Time.fixedDeltaTime;

        float checkDist = moveStep.magnitude + _config.WallOffset;

        // === STEP LOGIC ===
        Vector3 lowOrigin  = Rb.position + Vector3.up * _lowOriginMultiplier;
        Vector3 highOrigin = Rb.position + Vector3.up * _config.StepHeight;

        bool hitLow = Physics.Raycast(
            lowOrigin,
            moveDir,
            out RaycastHit lowHit,
            checkDist,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        bool hitHigh = Physics.Raycast(
            highOrigin,
            moveDir,
            checkDist
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

    [SerializeField] private float _lowOriginMultiplier;


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
            newPos.y -= _config.FallingSpeed * 5f * Time.fixedDeltaTime;
            transform.position = newPos;
            return;
        }
        
        newPos.x += MoveInput.x * _config.RotateSpeed * Time.fixedDeltaTime;
        
        newPos.x = Mathf.Clamp(newPos.x, _levelBounds.LeftX, _levelBounds.RightX);
        if (!IsBusted) {
            newPos.z += _config.SpeedForce * Time.fixedDeltaTime;
            newPos.y -= _config.FallingSpeed * Time.fixedDeltaTime;
        }
        else {
            float normalizedTime = ExpandedTime / SegmentDuration;
            
            float height = CurrentCurve.Evaluate(normalizedTime) * _config.JumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, normalizedTime);
            ExpandedTime += Time.fixedDeltaTime;
            if (ExpandedTime >= SegmentDuration) {
                IsBusted = false;
            }
        }
        transform.position = newPos;
    }


    [SerializeField] private float _getObjectsCooldownSeconds;
    public bool ObjectGetAllow { get; private set; } = true;

    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        if (!ObjectGetAllow) return;

        _visual.SetBoosted();
        CurrentCurve = curve;
        ExpandedTime = 0f;
        IsBusted = true;
        _initialPos = transform.position;
        TargetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, TargetPos);
        SegmentDuration = distance / _config.SpeedForce; 
        ObjectGetAllow = false;
        StartCoroutine(ObjectAllowCooldown());
        SetBoost?.Invoke();
    }

    private IEnumerator ObjectAllowCooldown() {
        yield return new WaitForSeconds(_getObjectsCooldownSeconds);
        ObjectGetAllow = true;
    } 

  

    private void VisualRotate() {
        float targetRoll = -MoveInput.x * _config.MaxRotate;

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


    public Vector3 GetPlayerPositionAt(float t) {
        // t ∈ [0..1], прогресс по кривой
        float height = CurrentCurve.Evaluate(t) * _config.JumpHeight;
        float y = Mathf.Lerp(_initialPos.y, TargetPos.y, t) + height;
        float z = Mathf.Lerp(_initialPos.z, TargetPos.z, t);

        float x = transform.position.x; // фиксируем X в момент старта
        return new Vector3(x, y, z);
    }
    
    

    
}
