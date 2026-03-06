using System;
using System.Collections;
using System.Collections.Generic;
using Architecture_M;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class Basketball : MonoBehaviour {
    [SerializeField] private TMP_Text _scoreText;
    
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
    private float _targetBounceHeight;

    [Inject] private Money2dSpawner _money2dSpawner;
    [Inject] private PlayerBank _bank;
    [Inject] private UpgradesCalculator _upgradesCalculator;

    
    
    private void Awake() {
        _parentBall.position = _spawnPointTransform.position;
        
        _hoopPosition = _hoop.position;
        _currentReward = _rewardForScore;
        _rb.useGravity = false;
        _targetBounceHeight = _spawnPointTransform.position.y;
    }



    private void Start() {
        _scoreText.text = _gameSave.GetSave.CountBaskets.ToString();
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
        if (collider.TryGetComponent(out PlayerMovement _)) {
            if(_allowToCick){
                _allowToCick = false;
                SetBouncy(false);
                KickBall(false);
            }
        }
        if (collider.TryGetComponent(out BotStateManager _)) {
            if(_allowToCick){
                _allowToCick = false;
                SetBouncy(false);
                KickBall(true);
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.contacts[0].normal.y > 0.5f) // удар о пол
        {
            float g = Mathf.Abs(Physics.gravity.y * _gravityMultiplier);

            float neededVelocity = Mathf.Sqrt(2f * g * _targetBounceHeight);

            Vector3 v = _rb.linearVelocity;
            v.y = neededVelocity;

            _rb.linearVelocity = v;
        }
    }

    private void KickBall(bool botKicked) {
        if (Random.value < _chanceToGoal) {
            ThrowInHoopAsync(_hoopPosition, !botKicked).Forget();
        }
        else {
            ThrowInHoopAsync(GetShieldPosition(), false).Forget();;
        }
    }

    private async UniTask ThrowInHoopAsync(Vector3 position, bool isRewarded) {
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
     

        float z = _shieldRenderer.bounds.max.z;
        float y = Random.Range(_shieldRenderer.bounds.max.y,  _shieldRenderer.bounds.min.y);
        float x = Random.Range(_shieldRenderer.bounds.max.x,  _shieldRenderer.bounds.min.x);
        
        newPos = new Vector3(x, y, z);
        
        return newPos;
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
        _parentBall.position = _spawnPointTransform.position;
        _rb.isKinematic = false;  // включаем обратно
        _allowToCick = true;
    } 
    
    [Inject] protected IGameSave<GameSavePC> _gameSave;
    
    private void GetMoneyReward() {
        print("Награда за попадание: " + _rewardForScore);
        _bank.AddMoney(_currentReward * _upgradesCalculator.GetUpgradeMultiplierByLevel());
        _gameSave.GetSave.CountBaskets++;
        _scoreText.text = _gameSave.GetSave.CountBaskets.ToString();
        
        _money2dSpawner.SpawnOneMoneyInPoint(transform.position);
        _confettiSpawner.SpawnConfetti();
        _currentReward+= _rewardForScore;
        
    }
    
    

    
}
