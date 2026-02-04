using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class Basketball : MonoBehaviour {
    
    [SerializeField] private Transform _parentBall;
    [SerializeField] private PhysicsMaterial _lowBounceMaterial;
    [SerializeField] private PhysicsMaterial _fullBounceMaterial;
    [SerializeField] private Collider _parentCollider;
    
    
    
    [SerializeField] private float _timeToRespawn;
    [SerializeField] private float _height;
    [SerializeField] private float _ballSpeed;
    [SerializeField] private Renderer _shieldRenderer;
    [SerializeField] private Transform _hoop; // тут именно центр нужен куда он залетит
    [SerializeField] private List<AnimationCurve> _kickTrajectories;
    [SerializeField] float _gravityMultiplier = 2f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _spawnPointTransform;
    
    [SerializeField] private float _rewardForScore = 100f;
    [Range(0f,1f), SerializeField] private float _chanceToGoal = 0.5f;
    
    
    [SerializeField] private ConfettiSpawner _confettiSpawner;
    private float _currentReward;
    
    
    private Vector3 _hoopPosition;
    private Vector3 _ballPositionSpawn;
    private PlayerBank _bank;


    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject]
    private void Init(PlayerBank bank) {
        _bank = bank;
    }
    
    
    private void Awake() {
        _ballPositionSpawn = _spawnPointTransform.position;
        _parentBall.position = _ballPositionSpawn;
        
        _hoopPosition = _hoop.position;
        _currentReward = _rewardForScore;
    }
    
    

    private void FixedUpdate() {
        _rb.AddForce(
            Physics.gravity * _gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    private bool _allowToCick = true;
    
        
    private void SetBouncy(bool on) {
        _parentCollider.sharedMaterial = on ? _fullBounceMaterial : _lowBounceMaterial;
    }
    
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerMovement player) || collider.TryGetComponent(out BotStateManager bot)) {
            if(_allowToCick){
                _allowToCick = false;
                SetBouncy(false);
                KickBall();
            }
        }
    }

    private void KickBall() {
        if (Random.value < _chanceToGoal) {
            ThrowInHoopAsync(_hoopPosition, true).Forget();
        }
        else {
            ThrowInHoopAsync(GetShieldPosition()).Forget();;
        }
    }

    private async UniTask ThrowInHoopAsync(Vector3 position, bool isRewarded = false) {
        float distance = Vector3.Distance(_parentBall.position, position);
        float timeToFlight = distance / _ballSpeed;
        Debug.Log("Дистанция до кольца: " + distance);
        Debug.Log("timeToFlight: " + timeToFlight);
        Debug.Log("position: " + position);
        
        
        float elapsedTime = 0f;
        AnimationCurve kickTrajectory = _kickTrajectories[Random.Range(0, _kickTrajectories.Count)];
        Vector3 initPos = _parentBall.position;
        while (elapsedTime < timeToFlight) {
            Vector3 newPos = _parentBall.position;
            float normalizedTime =  elapsedTime / timeToFlight; 
            
            float height = kickTrajectory.Evaluate(normalizedTime) * _height; // По высоте подымается
            
            newPos.x = Mathf.Lerp(initPos.x, position.x, normalizedTime);
            newPos.y = Mathf.Lerp(initPos.y, position.y, normalizedTime) + height;
            newPos.z = Mathf.Lerp(initPos.z, position.z, normalizedTime);

            _parentBall.position = newPos;
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        StartCoroutine(RespawnRoutine(_timeToRespawn, isRewarded));
    }

    private Vector3 newPos;
    private Vector3 GetShieldPosition() {
        Debug.Log("Shield x: " + _shieldRenderer.bounds.max.x);
        Debug.Log("Shield y: " + _shieldRenderer.bounds.max.y);
        Debug.Log("Shield z: " + _shieldRenderer.bounds.max.z);

        float z = _shieldRenderer.bounds.max.z;
        float y = Random.Range(_shieldRenderer.bounds.max.y,  _shieldRenderer.bounds.min.y);
        float x = Random.Range(_shieldRenderer.bounds.max.x,  _shieldRenderer.bounds.min.x);
        
        newPos = new Vector3(x, y, z);
        
        return newPos;
    }
    
    private void OnDrawGizmos() {
        if (_shieldRenderer == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(newPos, 0.5f); // точка в нужном месте
    }
    
    

    private IEnumerator RespawnRoutine(float timeToRespawn, bool isRewarded) {
        if (isRewarded) {
            GetMoneyReward();
        }
        yield return new WaitForSeconds(timeToRespawn);
        SetBouncy(true);
        _rb.linearVelocity = Vector3.zero;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        
        
        _rb.isKinematic = true;   // временно выключаем физику
        _parentBall.position = _ballPositionSpawn;
        _rb.isKinematic = false;  // включаем обратно
        
        
        _allowToCick = true;
        
    } 
    
    private void GetMoneyReward() {
        print("Награда за попадание: " + _rewardForScore);
        _bank.AddMoney(_currentReward);
        _money2dSpawner.SpawnOneMoneyInPoint(transform.position);
        _confettiSpawner.SpawnConfetti();
        _currentReward+= _rewardForScore;
        
    }
    
    

    
}
