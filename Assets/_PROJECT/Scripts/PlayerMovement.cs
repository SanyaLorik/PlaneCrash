using System;
using System.Security.Cryptography;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _speedForce = 10f;
    [SerializeField] private float _angle = 30f;

    [SerializeField] private Transform _skinTransform;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private GameObject _cruiserPrefab;
    
    
    [SerializeField] private AnimationCurve _fallCurve;
    
        
    [SerializeField] private float _totalFallTime = 7f;
    [SerializeField] private float _fallMultipluer;
    [SerializeField] private float maxFallSpeed = 2f;
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private float _maxRotate = 20f;
    [Range(0,3), SerializeField] private float _bustDuration;
    [Range(0,1), SerializeField] private float _spawnCraiserProp;
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    float _currentRoll;
    private float _fallProgress;

    private float _startY;
    private float _endY = 0f;
    private GameObject _cruiser;
    
    public bool Grounded { get; private set; }
    
    private enum VerticalState {
        Falling,
        Boosting
    }

    private VerticalState _verticalState = VerticalState.Falling;
    

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _startY =  transform.position.y;
        ContainFallTime();
        Debug.Log(_totalFallTime);
        SpawnCruiser();
    }
    
    private void Update() {
        Move();
        VisualRotate();
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<IBoostObject>(out var boostObject)) {
            Debug.Log(boostObject);
            boostObject.ApplyBoost(this);
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        _moveInput =  context.ReadValue<Vector2>();
    }
    
    private void Move() {
        if (Grounded) {
            return;
        }
        Vector3 newPos =  transform.position;
        newPos.z += _speedForce * Time.deltaTime;
        newPos.x += _moveInput.x * _rotateSpeed * Time.deltaTime;

        // Падение
        if (_verticalState == VerticalState.Falling) {
            float currentY = Mathf.Lerp(_startY, _endY, _fallCurve.Evaluate(_fallProgress));
            newPos.y = currentY;
            _fallProgress += Time.deltaTime / _totalFallTime;
            _fallProgress =  Mathf.Clamp01(_fallProgress);
        }
        
        if (_fallProgress == 1) {
            Grounded  = true;
        }
        
        transform.position = newPos;
    }

    public void ApplyVerticalBoost(float extraHeight) {
        _verticalState = VerticalState.Boosting;
        transform.DOKill(); 

        transform.DOMoveY(
                transform.position.y + extraHeight,
                _bustDuration
            )
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                _startY = transform.position.y; // текущая высота после буста
                _fallProgress = 0f; // начинаем кривое падение с этой точки
                
                ContainFallTime();
                SpawnCruiser();
                
                _verticalState = VerticalState.Falling;
            });
    }

    private void SpawnCruiser() {
        if (Random.value < _spawnCraiserProp) {
            return;
        }
        float landingZ = transform.position.z + _speedForce * _totalFallTime;
        Vector3 spawnCoord = new Vector3(transform.position.x, 0f, landingZ);

        if (_cruiser != null) {
            Destroy(_cruiser);
        }
        _cruiser = Instantiate(_cruiserPrefab, spawnCoord, _cruiserPrefab.transform.rotation);
    }
    

        
    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _maxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _rotateSpeed);

        Vector3 euler = _skinTransform.localEulerAngles;
        euler.z = _currentRoll;
        _skinTransform.localEulerAngles = euler;
    }



    private void ContainFallTime() {
        float height = _startY;
        _totalFallTime =  height / _speedForce * _fallMultipluer;
    }


}
