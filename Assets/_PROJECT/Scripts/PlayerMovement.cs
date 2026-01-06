using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _speedForce = 10f;
    [SerializeField] private float _walkSpeed = 10f;
    [SerializeField] private float _fallingSpeed = 7f;
    [SerializeField] private Transform _skinTransform;
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private float _maxRotate = 20f;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private float _jumpHeight;
    public AnimationCurve currentCurve;
    
    private float segmentDuration;
    private float expandedTime = 0;
    private Vector3 initial;
    private Vector3 target;
    private bool _isBusted;
    
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    private float _currentRoll;

    private CancellationTokenSource _playerCTS;
    public event Action<PlayerState> OnStateChange;
    
    public enum PlayerState {
        Flight,
        Walking
    }
    public PlayerState _playerState;
    
    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _playerCTS = new CancellationTokenSource();
        ChangeState(PlayerState.Walking);
    }
    
    private void OnDestroy() {
        _playerCTS?.Cancel();
        _playerCTS?.Dispose();
    }
    
    
    
    private void Update() {
        if (_playerState == PlayerState.Walking) {
            Walk();
        }
        else {
            FlightLogic();
            VisualRotate();
        }

        if (transform.position.z > -10 && !_playerIsFlight) {
            _playerIsFlight =  true;
            ChangeState(PlayerState.Flight);
        }
    }

    private bool _playerIsFlight;
    private void ChangeState(PlayerState playerState) {
        _playerState = playerState;
        OnStateChange?.Invoke(playerState);
        if (playerState == PlayerState.Flight) {
            PlayerRotateLocalX(-25, playerState);
        }
        else {
            PlayerRotateLocalX(-80, playerState);
        }
    }

    private async UniTask PlayerRotateLocalX(float targetAngleX, PlayerState playerState) {
        float duration = 1f;
    
        // Работаем с локальными углами (относительно родителя)
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
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
        
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
        
            await UniTask.Yield(_playerCTS.Token);
        }
    
        transform.localRotation = targetRot;
    }

    public void OnMove(InputAction.CallbackContext context) {
        _moveInput =  context.ReadValue<Vector2>();
    }

    private void Walk() {
        
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

        transform.position += move * _walkSpeed * Time.deltaTime;
        
        
        
        if (move.sqrMagnitude > 0.0001f) {
            float targetY = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;

            float y = Mathf.LerpAngle(
                transform.eulerAngles.y,
                targetY,
                _rotateSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x, // X НЕ ТРОГАЕМ
                y,                        // крутится только Y
                transform.eulerAngles.z  // Z НЕ ТРОГАЕМ
            );
        }

    }
    
    
    private void FlightLogic() {
        if (_groundChecker.Grounded) {
            return;
        }

        Vector3 newPos =  transform.position;
        newPos.x += _moveInput.x * _rotateSpeed * Time.deltaTime;

        if (!_isBusted) {
            newPos.z += _speedForce * Time.deltaTime;
            newPos.y -= _fallingSpeed * Time.deltaTime;
        }
        else {
            float normalizedTime = expandedTime / segmentDuration;
            
            float height = currentCurve.Evaluate(normalizedTime) * _jumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(initial.y, target.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(initial.z, target.z, normalizedTime);
            expandedTime += Time.deltaTime;
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
        segmentDuration = distance / _speedForce; 
    }

    
  
    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _maxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _rotateSpeed);

        Vector3 euler = _skinTransform.localEulerAngles;
        euler.z = _currentRoll;
        _skinTransform.localEulerAngles = euler;
    }
    
}
