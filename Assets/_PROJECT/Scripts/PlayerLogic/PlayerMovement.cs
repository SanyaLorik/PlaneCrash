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
    [SerializeField] private Collider _playerCollider; // можно удалить, CharacterController сам будет коллайдером
    
    private CharacterController _controller; // <-- НОВОЕ
    private Quaternion _defaultModelRotation;
    private Rigidbody _rb;
    public Transform Transform => transform;
    public Rigidbody Rb => _rb;

    public Vector2 MoveInput => _inputDirection2.Direction2;
    private float _currentRoll;
    private float _rollVelocity;
    public bool IsBusted { get; private set; }
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
    
    // Для гравитации и прыжков
    private float _verticalVelocity;
    private int _jumpsUsed;
    private bool _isGrounded;
    
    
    
    
    private void OnEnable()
    {
        _stateManager.ChangeState += ChangeSpaceRotation;
        _inputJumping.OnJumped += OnJump;
    }

    private void OnDisable()
    {
        _stateManager.ChangeState -= ChangeSpaceRotation;
        _inputJumping.OnJumped -= OnJump;
    }
    
    
    public void AddVerticalImpulse(float force)
    {
        _verticalVelocity = force;
        _jumpsUsed = 0;
    }
    
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Добавляем Character Controller, если его нет
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            _controller = gameObject.AddComponent<CharacterController>();
        }
    }
    
    private void Start()
    {
        ChangeSpaceRotation(PlayerState.Walking);
        TpPlayerInSpawn();
        _defaultModelRotation = _transformForRotate.localRotation;
    }
    
    private void Update()
    {
        if (_stateManager.CurrentState == PlayerState.Flight)
        {
            VisualRotate();
        }
    }
    
    private void FixedUpdate()
    {
        if (_stateManager.CurrentState == PlayerState.Walking ||
            _stateManager.CurrentState == PlayerState.TrampolineJumping)
        {
            Walk();
        }
        else if (_stateManager.CurrentState == PlayerState.Flight)
        {
            FlightLogic();
        }
    }
    
    public void TpPlayerInSpawn()
    {
        _stateManager.ChangePlayerState(PlayerState.Walking);
    
        _controller.enabled = false;
        transform.position = _levelBounds.PlayerSpawnPoint.position;
        _controller.enabled = true;
    
        _verticalVelocity = 0; // Сброс скорости
        _jumpsUsed = 0; // Сброс прыжков
        _visual.TeleportParticles();
    }
    
    public void TpPlayerInBetZone()
    {
        _controller.enabled = false;
        transform.position = _levelBounds.BetZonePosition.position;
        _controller.enabled = true;
        
        _verticalVelocity = 0;
        _visual.TeleportParticles();
    }
    
    public bool TryToKill()
    {
        _currentLifesCount--;
        _visual.StartDizzy();
        if (_currentLifesCount <= 0)
        {
            SetPlayerIsBombed();
        }
        return _currentLifesCount <= 0;
    }
    
    private void ResetLifes() => _currentLifesCount = _upgradesCalculator.GetDefenceByLevel();
    
    private void SetPlayerIsBombed()
    {
        IsBusted = false;
        IsBombed = true;
    }
    
    private void ChangeSpaceRotation(PlayerState playerState)
    {
        _tokenSource = new CancellationTokenSource();
        if (playerState == PlayerState.Flight)
        {
            ResetLifes();
            _controller.enabled = false;
            IsBombed = false;
            RotateLocalXAsync(30, playerState, _tokenSource.Token).Forget();
        }
        
        else if (playerState == PlayerState.Grounded || playerState == PlayerState.Cruisered)
        {
            RotateLocalXAsync(0, playerState, _tokenSource.Token).Forget();
            ResetModelRotation();
        }

        if (playerState == PlayerState.Walking) {
            _controller.enabled = true;
        }
    }
    
    private void ResetModelRotation()
    {
        _transformForRotate.localRotation = _defaultModelRotation;
    }
    
    private async UniTask RotateLocalXAsync(float TargetPosAngleX, PlayerState playerState, CancellationToken token)
    {
        float duration = 1f;
        
        Vector3 currentLocalEuler = transform.localEulerAngles;
        Vector3 TargetPosLocalEuler;
        if (playerState == PlayerState.Walking)
        {
            TargetPosLocalEuler = new Vector3(TargetPosAngleX, currentLocalEuler.y, currentLocalEuler.z);
        }
        else
        {
            TargetPosLocalEuler = new Vector3(TargetPosAngleX, 0f, 0f);
        }
        
        Quaternion startRot = transform.localRotation;
        Quaternion TargetPosRot = Quaternion.Euler(TargetPosLocalEuler);
        
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
            
            transform.localRotation = Quaternion.Slerp(startRot, TargetPosRot, t);
            
            await UniTask.Yield(token);
        }
        
        transform.localRotation = TargetPosRot;
    }
    
    public void OnJump()
    {
        if (_stateManager.CurrentState != PlayerState.Walking) return;
        
        if (_jumpsUsed == 0)
        {
            _verticalVelocity = _config.JumpForce;
            _jumpParticlesController.Play();
            OnJumpPressed?.Invoke();
            Debug.Log("OnJumpPressed?.Invoke();");
            _jumpsUsed = 1;
        }
        else if (_jumpsUsed == 1)
        {
            _verticalVelocity = _config.SecondJumpForce;
            OnDoubleJumpPressed?.Invoke();
            Debug.Log("OnDoubleJumpPressed?.Invoke();");
            _jumpParticlesController.Play();
            _jumpsUsed = 2;
        }
    }
    
    private bool _wasGroundedLastFrame = false; // <-- ЭТА ПЕРЕМЕННАЯ НУЖНА

    private void Walk()
    {
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
            OnRunningStateChanged?.Invoke(IsRunning);
        }

        // ГРАВИТАЦИЯ
        if (!_controller.isGrounded)
        {
            _verticalVelocity += Physics.gravity.y * _config.GravityScale * Time.fixedDeltaTime;
        }

        Vector3 horizontalMove = hasInput
            ? move.normalized * _config.WalkSpeed * Time.fixedDeltaTime
            : Vector3.zero;

        Vector3 verticalMove = Vector3.up * _verticalVelocity * Time.fixedDeltaTime;

        _controller.Move(horizontalMove + verticalMove);

        // Проверяем grounded ПОСЛЕ Move
        _isGrounded = _controller.isGrounded;

        // Прилипание к земле (анти-дребезг)
        if (_isGrounded)
        {
            if (_verticalVelocity < 0f)
                _verticalVelocity = -2f; // маленький прижим
        }

        // Настоящее приземление
        bool justLanded = _isGrounded && !_wasGroundedLastFrame && _verticalVelocity < -0.1f;

        if (justLanded)
        {
            Debug.Log($"LANDED! Frame: {Time.frameCount}");
            _jumpsUsed = 0;
            Floored?.Invoke();
            _stateManager.ChangePlayerState(PlayerState.Walking);
        }

        _wasGroundedLastFrame = _isGrounded;

        if (hasInput)
        {
            WalkRotate(move);
        }
    }
    
    private void WalkRotate(Vector3 move)
    {
        if (move.sqrMagnitude > 0.0001f)
        {
            float TargetPosY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            
            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                TargetPosY,
                _config.RotateSpeed * Time.fixedDeltaTime
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                y,
                transform.eulerAngles.z
            );
        }
    }
    
    private void FlightLogic()
    {
        Vector3 newPos = transform.position;
        if (IsBombed)
        {
            newPos.y -= _config.FallingSpeed * 5f * Time.fixedDeltaTime;
            transform.position = newPos;
            return;
        }
        
        newPos.x += MoveInput.x * _config.RotateSpeed * Time.fixedDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, _levelBounds.LeftX, _levelBounds.RightX);
        
        if (!IsBusted)
        {
            newPos.z += _config.SpeedForce * Time.fixedDeltaTime;
            newPos.y -= _config.FallingSpeed * Time.fixedDeltaTime;
        }
        else
        {
            float normalizedTime = ExpandedTime / SegmentDuration;
            
            float height = CurrentCurve.Evaluate(normalizedTime) * _config.JumpHeight;
            newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, normalizedTime);
            ExpandedTime += Time.fixedDeltaTime;
            if (ExpandedTime >= SegmentDuration)
            {
                IsBusted = false;
            }
        }
        
        // Для Flight режима используем прямой transform, потому что физика не нужна
        transform.position = newPos;
    }
    
    [SerializeField] private float _getObjectsCooldownSeconds;
    public bool ObjectGetAllow { get; private set; } = true;
    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost)
    {
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
    
    private IEnumerator ObjectAllowCooldown()
    {
        yield return new WaitForSeconds(_getObjectsCooldownSeconds);
        ObjectGetAllow = true;
    }
    
    [SerializeField] private Transform _transformForRotate;
    
    private void VisualRotate()
    {
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
    
    private void SetModelRotation(Vector3 euler)
    {
        euler.y = _currentRoll;
        _transformForRotate.localEulerAngles = euler;
    }
    
    public Vector3 GetPlayerPositionAt(float t)
    {
        float height = CurrentCurve.Evaluate(t) * _config.JumpHeight;
        float y = Mathf.Lerp(_initialPos.y, TargetPos.y, t) + height;
        float z = Mathf.Lerp(_initialPos.z, TargetPos.z, t);
        float x = transform.position.x;
        return new Vector3(x, y, z);
    }
}