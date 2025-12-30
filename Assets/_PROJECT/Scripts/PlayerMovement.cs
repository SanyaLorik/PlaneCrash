using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float airControlForce = 10f;
    
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _angle = 30f;

    [SerializeField] private Transform _skinTransform;
    [SerializeField] private float _maxRotate = 30f;
    [SerializeField] private float _rotateSpeed = 6f;

    [SerializeField] private float _glideGravityFactor = 0.7f;
    [SerializeField] float maxFallSpeed = 6f;
    
    [SerializeField] private float _constantSpeed; // скорость которую хочим
    
    [SerializeField] LayerMask _groundMask;
    
    
    private Rigidbody _rb;
    private Vector2 _moveInput;
    float currentRoll;
    public bool grounded = false;
 
    
    
    
    private void Awake() {
        _rb =  GetComponent<Rigidbody>();
    }
    
    private void Start() {
        FirstFlight();
        // _rb.linearDamping = 0.1f;
    }
    

    private void FixedUpdate() {
        FlyingRotate();
        GravityCompensation();
        LimitationFallingSpeed();
        NormalizeSpeed();
        CheckGrounded();
    }

    private void CheckGrounded() {
        float groundedCheckDistance = 0.2f;
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundedCheckDistance,
            _groundMask
        );
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (grounded) {
            Debug.Log("Вы разбились нахуй!");
            _rb.linearDamping = 0.5f;
        }
    }
    
    private void Update() {
        VisualRotate();
    }
    
    
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.TryGetComponent<IBoostObject>(out var boostObject)) {
            Debug.Log(boostObject);
            boostObject.ApplyBoost(this);
        }
    }

    public void AddVerticalVelocity(float forceBoost) {
        _rb.AddForce(Vector3.up *  forceBoost, ForceMode.VelocityChange);
    }

    
    // Постоянная скорость
    private void NormalizeSpeed() {
        if (grounded) {
            return;
        }
        
        // Чисто горизонтальное движение
        Vector3 currentHorizontal = Vector3.ProjectOnPlane(_rb.linearVelocity, Vector3.up);
        
        Vector3 targetHorizontal = transform.forward * _constantSpeed;

        Vector3 correction = targetHorizontal - currentHorizontal;
        _rb.AddForce(correction, ForceMode.Acceleration);
    }

    private void GravityCompensation() {
        // Добавление импульса при падении
        if (_rb.linearVelocity.y < 0) {
            Vector3 antigravity = - Physics.gravity * _rb.mass * _glideGravityFactor;
            _rb.AddForce(antigravity, ForceMode.Force);
        }
    }

    // Максимальная скорость падения, можно регулировать чтоб не так быстро разьебывался
    private void LimitationFallingSpeed() {
        if (_rb.linearVelocity.y < -maxFallSpeed) {
            _rb.linearVelocity = new Vector3(
                _rb.linearVelocity.x,
                -maxFallSpeed,
                _rb.linearVelocity.z);
        }
    }




    private Vector3 direction;
    private float rad;
    private void FirstFlight() {
        rad = _angle * Mathf.Deg2Rad; 
        direction = new Vector3(0, Mathf.Sin(rad), Mathf.Cos(rad));
        _rb.linearVelocity = Vector3.Project(_rb.linearVelocity, direction);
        _rb.AddForce(direction * _force, ForceMode.Impulse);
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
        VisualRotate();
    }

    
    private void FlyingRotate() {
        float steer = _moveInput.x;

        Vector3 right = transform.right;

        _rb.AddForce(right * steer * airControlForce, ForceMode.Force);
    }

    


    

    
    
    
}
