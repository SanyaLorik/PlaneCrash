using System;
using System.Collections;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _speedForce = 10f;
    [SerializeField] private float _fallingSpeed = 7f;
    [SerializeField] private Transform _skinTransform;
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private float _maxRotate = 20f;
    [SerializeField] private GroundChecker _groundChecker;
    
    
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    float _currentRoll;

    
    
    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }
    
    private void Update() {
        Move();
        VisualRotate();
    }


    public void OnMove(InputAction.CallbackContext context) {
        _moveInput =  context.ReadValue<Vector2>();
    }
    
    
    public AnimationCurve currentCurve;


    private bool isBusted;
    private void Move() {
        if (_groundChecker.Grounded) {
            return;
        }

        Vector3 newPos =  transform.position;
        if (!isBusted) {
            newPos.z += _speedForce * Time.deltaTime;
            newPos.y -= _fallingSpeed * Time.deltaTime;
        }
        newPos.x += _moveInput.x * _rotateSpeed * Time.deltaTime;

        if (isBusted) {
            float normalizedTime = expandedTime / segmentDuration;
            
            float height = currentCurve.Evaluate(normalizedTime) * _jumpHeight; // По высоте подымается
            newPos.y = Mathf.Lerp(initial.y, target.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(initial.z, target.z, normalizedTime);
            expandedTime += Time.deltaTime;
            if (expandedTime >= segmentDuration) {
                isBusted = false;
            }
        }
        transform.position = newPos;
    }


    [SerializeField] private float _jumpHeight;
    private float segmentDuration;
    private float expandedTime = 0;
    private Vector3 initial;
    private Vector3 target;
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        currentCurve = curve;
        expandedTime = 0f;
        isBusted = true;
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
