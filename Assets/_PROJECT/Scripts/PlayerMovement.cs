using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _speedForce = 10f;
    [SerializeField] private float _angle = 30f;

    [SerializeField] private Transform _skinTransform;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] private GameObject _cruiserPrefab;
    
    
        
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private float _maxRotate = 20f;
    [Range(0,3), SerializeField] private float _bustDuration;
    
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
        boostStartTime = Time.time;
    }
    
    private void Update() {
        Move();
        VisualRotate();
    }
    
    // private void OnTriggerEnter(Collider other) {
    //     if (other.gameObject.TryGetComponent<IBoostObject>(out var boostObject)) {
    //         Debug.Log(boostObject);
    //         boostObject.ApplyBoost(this);
    //     }
    // }

    public void OnMove(InputAction.CallbackContext context) {
        _moveInput =  context.ReadValue<Vector2>();
    }
    
    
    // Допустим 1 кривая условно на него сразу действует буст
    [SerializeField] private AnimationCurve boostCurve;
    [SerializeField] private  float boostDuration = 10f;
    private bool isBusted = true;
    private float boostStartTime;

    [SerializeField] private float _curveMultiplyer;

    private void Move() {

        Vector3 newPos =  transform.position;
        newPos.z += _speedForce * Time.deltaTime;
        newPos.x += _moveInput.x * _rotateSpeed * Time.deltaTime;


        // работа буста
        if (isBusted) {
            // Сколько времени прошло с начала буста
            float elapsedTime = Time.time - boostStartTime;
            float normalizedTime = elapsedTime / boostDuration;
            Debug.Log(normalizedTime);
            if (normalizedTime <= 1f) {
                // Получаем значение кривой в этот момент времени
                float curveValue = boostCurve.Evaluate(normalizedTime);
                newPos.y = _startY + curveValue * _curveMultiplyer;
                Debug.Log(newPos.y);
            }
            else {
                isBusted = false;
            }

            
        }
        
        
        transform.position = newPos;
    }
    
    


    private void SpawnCruiser() {
        // _cruiser = Instantiate(_cruiserPrefab, spawnCoord, _cruiserPrefab.transform.rotation);
    }
    

        
    private void VisualRotate() {
        float targetRoll = -_moveInput.x * _maxRotate;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _rotateSpeed);

        Vector3 euler = _skinTransform.localEulerAngles;
        euler.z = _currentRoll;
        _skinTransform.localEulerAngles = euler;
    }





}
