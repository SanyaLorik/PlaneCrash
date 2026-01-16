using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotFlightLogic : MonoBehaviour {
    [SerializeField] private int _countBoostEquilizeSpeed;
    [SerializeField] private float _botSpeedCorrect;
    [SerializeField] private Vector3 _flightPosition;
    [SerializeField] private Transform _botModelForRotate;
    [Range(0,1), SerializeField] private float _trueWayChance;
    private PlayerStateManager _stateManager;
    private PlayerConfig _playerConfig;
    private BoostSpawner _boostSpawner;
    private List<Boost> _boostWay;
   
    
    private AnimationCurve _currentCurve;
    private float _segmentDuration;
    private float _expandedTime = 0;
    private Vector3 _initialPos;
    private Vector3 _targetPos;
    private int _countGetBoosts;
    private BotBrain _botBrain;
    
    [Inject]
    public void Init(PlayerStateManager stateManager, BoostSpawner boostSpawner, PlayerConfig playerConfig) {
        
        _stateManager = stateManager;
        _stateManager.ChangeState += OnChangeState;
        _boostSpawner = boostSpawner;
        _playerConfig = playerConfig;
    }

    private void Awake() {
        _botBrain = GetComponent<BotBrain>();
    }
    


    private void OnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _botBrain.StopBotEblaning();
            TpBotNearPlayer();
            // Сбросить кол-во бустов надо
            PlayerRotateLocalX(-25);
            ResetCountBoosts();
            Debug.Log("Вызов GetRandomWay");
            _boostWay = _boostSpawner.GetRandomWay(_trueWayChance);
            if (_boostWay.Count == 0) {
                Debug.LogError("Цепочка бустов пустая!");
                return;
            }
            Debug.Log("Кол-во бустов у бота: " + _boostWay.Count);
            SetBooster(_boostWay[0].randomTrajectory, _boostWay[0].transform.position);
            BotFlightCycle();
        }
    }


    private async UniTask BotFlightCycle() {
        Debug.Log(_boostWay.Count);
        while (_countGetBoosts <= _boostWay.Count) {
            FlightLogic();
            await UniTask.WaitForFixedUpdate();
        }
        Debug.Log("Выход из цикла, transform.position.y: " + transform.position.y);
        // Чутка подождать пока полежит
        PlayerRotateLocalX(-80);
        await UniTask.Delay(2000);
        ResetEblaningLogic();
    }
    
    private void ResetEblaningLogic() {
        transform.position = _playerConfig.PlayerSpawnPosition;
        _botBrain.StartBotEblaning();
            
    }

    private void TpBotNearPlayer() {
        Vector3 flightPosition = _flightPosition;
        flightPosition.x += Random.Range(-7, 7);
        transform.position = flightPosition;
    }

    private void ResetCountBoosts() {
        _countGetBoosts = 0;
    }
    
    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        Debug.Log("Следующий буст бота: " + nextBoost);
        _currentCurve = curve;
        _expandedTime = 0f;
        _initialPos = transform.position;
        _targetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, _targetPos);
        _countGetBoosts++;
        if (_countGetBoosts < _countBoostEquilizeSpeed) {
            // Чуть бырее
            _segmentDuration = distance / (_playerConfig.SpeedForce + _botSpeedCorrect);
        }
        else {
            // Уравниваем скорость
            _segmentDuration = distance / _playerConfig.SpeedForce;
        }
    } 
    
    
    private void FlightLogic() {
        Vector3 newPos =  transform.position;
        

        float normalizedTime = _expandedTime / _segmentDuration;
            
        float height = _currentCurve.Evaluate(normalizedTime) * _playerConfig.JumpHeight; // По высоте подымается
        newPos.y = Mathf.Lerp(_initialPos.y, _targetPos.y, normalizedTime) + height;
        newPos.z = Mathf.Lerp(_initialPos.z, _targetPos.z, normalizedTime);
        newPos.x = Mathf.Lerp(_initialPos.x, _targetPos.x, normalizedTime);
        _expandedTime += Time.fixedDeltaTime;

        transform.position = newPos;
    }
    
    
    private async UniTask PlayerRotateLocalX(float _targetPosAngleX) {
        float duration = 1f;
    
        Vector3 _targetPosLocalEuler;
        _targetPosLocalEuler = new Vector3(_targetPosAngleX, 0f, 180f);
        
    
        Quaternion startRot = _botModelForRotate.localRotation;
        Quaternion _targetPosRot = Quaternion.Euler(_targetPosLocalEuler);
    
        float elapsedTime = 0;
    
        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
        
            _botModelForRotate.localRotation = Quaternion.Slerp(startRot, _targetPosRot, t);
        
            await UniTask.Yield();
        }
    
        _botModelForRotate.localRotation = _targetPosRot;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        Debug.Log("Поворот игрока: " + transform.localRotation);
    }

}
