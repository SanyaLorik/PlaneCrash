using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : MonoBehaviour {
    private PlayerConfig _config;

    [Inject]
    public void Init(PlayerConfig config) {
        _config = config;
    }
    
    private AnimationCurve currentCurve;
    private float segmentDuration;
    private float expandedTime = 0;
    private Vector3 initial;
    private Vector3 target;
    private bool _isBusted;
    
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private float _currentRoll;

    private CancellationTokenSource _playerCTS;
    private PlayerStateManager _stateManager;

    public float PlayerSpeed => _config.SpeedForce;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _stateManager = GetComponent<PlayerStateManager>();
        _rb.useGravity = true;
        _stateManager.OnChangeState += ChangeSpaceRotation;
    }
    
    private void Start() {
        _playerCTS = new CancellationTokenSource();
        ChangeSpaceRotation(PlayerState.Walking);
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


    private void ChangeSpaceRotation(PlayerState playerState) {
        if (playerState == PlayerState.Flight) {
            PlayerRotateLocalX(-25, playerState);
            _rb.useGravity = false;
        }
        else {
            PlayerRotateLocalX(-80, playerState);
            _rb.useGravity = true;
        }
    }

    private async UniTask PlayerRotateLocalX(float targetAngleX, PlayerState playerState) {
        float duration = 1f;
    
        Vector3 currentLocalEuler = transform.localEulerAngles;
        Vector3 targetLocalEuler;
        if (playerState == PlayerState.Walking) {
            targetLocalEuler = new Vector3(targetAngleX, currentLocalEuler.y, currentLocalEuler.z);
        }
        else {
            targetLocalEuler = new Vector3(targetAngleX, 0f, 0f);
        }
    
        Quaternion startRot = transform.localRotation;
        Quaternion targetRot = Quaternion.Euler(targetLocalEuler);
    
        float elapsedTime = 0;
    
        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
        
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
        
            await UniTask.Yield(_playerCTS.Token);
        }
    
        transform.localRotation = targetRot;
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
            _rb.AddForce(Vector3.up * _config.JumpForce, ForceMode.Impulse);
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
            float targetY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetY,
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
            float normalizedTime = expandedTime / segmentDuration;
            
            float height = currentCurve.Evaluate(normalizedTime) * _config.JumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(initial.y, target.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(initial.z, target.z, normalizedTime);
            expandedTime += Time.fixedDeltaTime;
            if (expandedTime >= segmentDuration) {
                _isBusted = false;
            }
        }
        transform.position = newPos;
    }


    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        currentCurve = curve;
        expandedTime = 0f;
        _isBusted = true;
        initial = transform.position;
        target = nextBoost;
        float distance = Vector3.Distance(initial, target);
        segmentDuration = distance / _config.SpeedForce; 
    } 

    
  
    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _config.MaxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.fixedDeltaTime * _config.RotateSpeed);

        Vector3 euler = transform.localEulerAngles;
        euler.z = _currentRoll;
        transform.localEulerAngles = euler;
    }
    
    
    private void OnDestroy() {
        _playerCTS?.Cancel();
        _playerCTS?.Dispose();
    }
    
}
