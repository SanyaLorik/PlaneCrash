using System;
using System.Security.Cryptography;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _speedForce = 10f;
    [SerializeField] private float _angle = 30f;

    [SerializeField] private Transform _skinTransform;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private GameObject _cruiserPrefab;
    
    
    [SerializeField] private AnimationCurve _fallCurve;
    
        
    [SerializeField] private float _totalFallTime = 7f;
    [SerializeField] private float maxFallSpeed = 2f;
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private float _maxRotate = 20f;
    
    private Rigidbody _rb;
    float currentRoll;
    private float _fallProgress;

    private float startY;
    private float endY = 0f;
    
    private Vector2 _moveInput;
    public bool grounded;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        startY =  transform.position.y;
        ContainFallTime();
        Debug.Log(_totalFallTime);
        SpawnCruiser();
    }


    private void Update() {
        Move();
        VisualRotate();
    }

    private enum VerticalState
    {
        Falling,
        Boosting
    }

    private VerticalState _verticalState = VerticalState.Falling;
    private void Move() {
        if (grounded) {
            return;
        }
        Vector3 newPos =  transform.position;
        newPos.z += _speedForce *  Time.deltaTime;
        newPos.x += _moveInput.x * _rotateSpeed * Time.deltaTime;

        if (_verticalState == VerticalState.Falling) {
            float currentY;
            if (_fallProgress == 0) {
                currentY = startY; // просто закрепляем текущую позицию
            }
            else {
                currentY = Mathf.Lerp(startY, endY, _fallCurve.Evaluate(_fallProgress));
            }
            newPos.y = currentY;
            _fallProgress += Time.deltaTime / _totalFallTime;
            _fallProgress =  Mathf.Clamp01(_fallProgress);
        }
        
        if (_fallProgress == 1) {
            grounded  = true;
            
        }
        
        // Rotate Change
        transform.position = newPos;
    }


    private GameObject _cruiser;
    private void SpawnCruiser() {
        float landingZ = transform.position.z + _speedForce * _totalFallTime;
        Vector3 spawnCoord = new Vector3(transform.position.x, 0f, landingZ);

        if (_cruiser != null) {
            Destroy(_cruiser);
        }
        _cruiser = Instantiate(_cruiserPrefab, spawnCoord, _cruiserPrefab.transform.rotation);
    }
    

        
    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _maxRotate;
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * _rotateSpeed);

        Vector3 euler = _skinTransform.localEulerAngles;
        euler.z = currentRoll;
        _skinTransform.localEulerAngles = euler;
    }

    public void OnMove(InputAction.CallbackContext context) {
        _moveInput =  context.ReadValue<Vector2>();
    }

    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<IBoostObject>(out var boostObject)) {
            Debug.Log(boostObject);
            boostObject.ApplyBoost(this);
        }
    }


    public void ApplyVerticalBoost(float extraHeight) {
        _verticalState = VerticalState.Boosting;
        transform.DOKill(); // ОЧЕНЬ ВАЖНО

        transform.DOMoveY(
                transform.position.y + extraHeight,
                1f
            )
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                startY = transform.position.y; // текущая высота после буста
                ContainFallTime();

                _fallProgress = 0f; // начинаем кривое падение с этой точки
                SpawnCruiser();
                _verticalState = VerticalState.Falling;
            });

    }


    private void ContainFallTime() {
        float height = startY;
        _totalFallTime =  height / _speedForce * 21;
    }





    
    

}
