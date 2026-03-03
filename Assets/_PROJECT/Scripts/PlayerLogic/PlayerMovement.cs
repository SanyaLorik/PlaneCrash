using System;
using System.Collections;
using System.Threading;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerMovement : FlightObject
{
    [SerializeField] private float _smoothTime = 0.3f;
    [SerializeField] private int _currentLifesCount;
    [SerializeField] private JumpParticlesController _jumpParticlesController;
    [SerializeField] private CharacterController _controller; // 
    [SerializeField] private float _getObjectsCooldownSeconds;
    [SerializeField] private float _angleToFlight; // 
    [SerializeField] private float _yFlyCorrectRotation = 5f; // из-за анимации надо чуит развернуть хуилу
    
    public Transform Transform => transform;

    
    public Vector2 MoveInput => _inputDirection2.Direction2;
    private float _currentRoll;
    private float _rollVelocity;
    private bool _isOnLift;
    public bool IsBusted { get; private set; }
    public bool IsBombed;
    

    
    public float PlayerSpeed => _config.SpeedForce;
    public float JumpHeight => _config.JumpHeight;
    public event Action SetBoost;
    public event Action JumpPressed;
    public event Action DoubleJumpPressed;
    public event Action<bool> RunningStateChanged;
    public event Action Floored;
    
    public bool IsRunning { get; private set; }
    
    [Inject] private PlayerConfig _config;
    [Inject] private UpgradesCalculator _upgradesCalculator;
    [Inject] private PlayerStateManager _stateManager;
    [Inject] private LevelBounds _levelBounds;
    [Inject] private PlayerVisual _visual;
    [Inject] private IInputDirection2 _inputDirection2;
    [Inject] private IInputJumping _inputJumping;
    
    // Для гравитации и прыжков
    private float _verticalVelocity;
    private int _jumpsUsed;
    public bool IsGrounded { get; private set; }


    private void OnEnable() {
        _stateManager.ChangeState += ChangeSpaceRotation;
        _inputJumping.OnJumped += OnJump;
    }

    private void OnDisable() {
        _stateManager.ChangeState -= ChangeSpaceRotation;
        _inputJumping.OnJumped -= OnJump;
    }
    
    
    public void AddVerticalImpulse(float force) {
        _verticalVelocity = force;
        _jumpsUsed = 0;
    }
    
    private void Start() {
        ChangeSpaceRotation(PlayerState.Walking);
        TpPlayerInSpawn();
    }
    
    private void Update() {
        if (_stateManager.CurrentState == PlayerState.Flight) {
            VisualRotate();
            FlightLogic();
        }
        else if (_stateManager.CurrentState == PlayerState.Walking ||
            _stateManager.CurrentState == PlayerState.TrampolineJumping) {
            Walk();
        }
    }
    
    public void TpPlayerInSpawn() {
        _stateManager.ChangePlayerState(PlayerState.Walking);
        TeleportBase(_levelBounds.PlayerSpawnPoint.position);
    }
    
    public void TpPlayerInBetZone() {
        TeleportBase(_levelBounds.BetZonePosition.position);
    }
    
    public void TpPlayerInPoint(Transform target) {
        TeleportBase(target.position);
    }

    private void TeleportBase(Vector3 point) {
        _controller.enabled = false;
        transform.position = point;
        _controller.enabled = true;
    
        _verticalVelocity = 0; // Сброс скорости
        _jumpsUsed = 0; // Сброс прыжков
        _visual.TeleportParticles();
    }


    
    public bool TryToKill() {
        Debug.Log("minus jizn");
        _currentLifesCount--;
        _visual.StartDizzy();
        if (_currentLifesCount <= 0)
        {
            SetPlayerIsBombed();
        }
        return _currentLifesCount <= 0;
    }
    
    private void ResetLifes() => _currentLifesCount = _upgradesCalculator.GetDefenceByLevel();
    
    private void SetPlayerIsBombed() {
        IsBusted = false;
        IsBombed = true;
    }
    
    private void ChangeSpaceRotation(PlayerState playerState) {
        _tokenSource = new CancellationTokenSource();
        if (playerState == PlayerState.Flight) {
            ResetLifes();
            IsBombed = false;
            RotateLocalXAsync(_angleToFlight, playerState, _tokenSource.Token).Forget();
        }
        
        else if (playerState == PlayerState.Grounded || playerState == PlayerState.Cruisered) {
            RotateLocalXAsync(0, playerState, _tokenSource.Token).Forget();
            ResetModelRotation();
        }
    }
    
   
    
    private async UniTask RotateLocalXAsync(float targetPosAngleX, PlayerState playerState, CancellationToken token) {
        float duration = 1f;
        
        Vector3 currentLocalEuler = transform.localEulerAngles;
        Vector3 TargetPosLocalEuler;
        if (playerState == PlayerState.Walking) {
            TargetPosLocalEuler = new Vector3(targetPosAngleX, currentLocalEuler.y, currentLocalEuler.z);
        }
        else {
            TargetPosLocalEuler = new Vector3(targetPosAngleX, _yFlyCorrectRotation, 0f);
        }
        
        Quaternion startRot = transform.localRotation;
        Quaternion TargetPosRot = Quaternion.Euler(TargetPosLocalEuler);
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            transform.localRotation = Quaternion.Slerp(startRot, TargetPosRot, t);
            
            await UniTask.Yield(token);
        }
        
        transform.localRotation = TargetPosRot;
    }
    
    public void OnJump() {
        if (_stateManager.CurrentState != PlayerState.Walking) return;
        
        if (_jumpsUsed == 0) {
            _verticalVelocity = _config.JumpForce;
            _jumpParticlesController.Play();
            JumpPressed?.Invoke();
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1) {
            _verticalVelocity = _config.SecondJumpForce;
            DoubleJumpPressed?.Invoke();
            _jumpParticlesController.Play();
            _jumpsUsed = 2;
        }
    }
    
    private bool _wasGroundedLastFrame = false;

    private Vector3 _externalMotion;

    public void AddExternalMotion(Vector3 motion, bool inLift = true) {
        _externalMotion += motion;
        if (inLift) {
            _verticalVelocity = 0;
        }
    }
    
        
    public void SetLiftState(bool value) {
        _isOnLift = value;

        if (value)
        {
            _verticalVelocity = 0f;
            _jumpsUsed = 0;
        }
    }

    private void Walk() {
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * MoveInput.x + camForward * MoveInput.y;
        bool hasInput = move.sqrMagnitude > 0.001f;

        if (hasInput != IsRunning)
        {
            IsRunning = hasInput;
            RunningStateChanged?.Invoke(IsRunning);
        }

        // ГРАВИТАЦИЯ
        if (!_controller.isGrounded && !_isOnLift) {
            _verticalVelocity += Physics.gravity.y * _config.GravityScale * Time.deltaTime;
        }
        

        Vector3 horizontalMove = hasInput
            ? move.normalized * _config.WalkSpeed * Time.deltaTime
            : Vector3.zero;

        Vector3 verticalMove = Vector3.up * _verticalVelocity * Time.deltaTime;

        _controller.Move(horizontalMove + verticalMove + _externalMotion);
        _externalMotion = Vector3.zero;
        // Проверяем grounded ПОСЛЕ Move
        IsGrounded = _controller.isGrounded || _isOnLift;

        // Прилипание к земле (анти-дребезг)
        if (IsGrounded)
        {
            if (_verticalVelocity < 0f)
                _verticalVelocity = -2f;
        }

        // Настоящее приземление
        bool justLanded = IsGrounded && !_wasGroundedLastFrame && _verticalVelocity < -0.1f;

        if (justLanded)
        {
            _jumpsUsed = 0;
            Floored?.Invoke();
            _stateManager.ChangePlayerState(PlayerState.Walking);
        }

        _wasGroundedLastFrame = IsGrounded;

        if (hasInput)
        {
            WalkRotate(move);
        }
    }
    
    private void WalkRotate(Vector3 move) {
        if (move.sqrMagnitude > 0.0001f) {
            float TargetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            
            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                TargetPosY,
                _config.RotateSpeed * Time.deltaTime
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                y,
                transform.eulerAngles.z
            );
        }
    }
    
    private void FlightLogic() {
        Vector3 newPos = transform.position;
        if (IsBombed) {
            newPos.y -= _config.FallingSpeed * 4f * Time.deltaTime;
            _controller.Move(newPos - transform.position);
            return;
        }
        
        newPos.x += MoveInput.x * _config.RotateSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, _levelBounds.LeftX, _levelBounds.RightX);
        
        if (!IsBusted) {
            newPos.z += _config.SpeedForce * Time.deltaTime;
            newPos.y -= _config.FallingSpeed * Time.deltaTime;
        }
        else {
            float normalizedTime = ExpandedTime / SegmentDuration;
            
            float height = CurrentCurve.Evaluate(normalizedTime) * _config.JumpHeight;
            newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, normalizedTime);
            ExpandedTime += Time.deltaTime;
            if (ExpandedTime >= SegmentDuration)
            {
                IsBusted = false;
            }
        }
        
        _controller.Move(newPos - transform.position);
    }
    
    public bool ObjectGetAllow { get; private set; } = true;
    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        if (!ObjectGetAllow || IsBombed) return;
        Debug.Log("Set boost: " + nextBoost);
        
        
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
            _smoothTime
        );
        Vector3 euler = _transformForRotate.localEulerAngles;
        SetModelRotation(euler);
    }
    
    private void SetModelRotation(Vector3 euler) {
        euler.z = _currentRoll;
        _transformForRotate.localEulerAngles = euler;
    }
    
    public Vector3 GetPlayerPositionAt(float t) {
        float height = CurrentCurve.Evaluate(t) * _config.JumpHeight;
        float x = transform.position.x;
        float y = Mathf.Lerp(_initialPos.y, TargetPos.y, t) + height;
        float z = Mathf.Lerp(_initialPos.z, TargetPos.z, t);
        return new Vector3(x, y, z);
    }
}