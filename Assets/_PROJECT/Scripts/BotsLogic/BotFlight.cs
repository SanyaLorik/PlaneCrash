using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BotFlight : MonoBehaviour, IBotBehaviour {
    [SerializeField] private int _countBoostEquilizeSpeed;
    [SerializeField] private float _botSpeedCorrect;
    [SerializeField] private Vector3 _flightPosition;
    [SerializeField] private Transform _botModelForRotate;
    [Range(0,1), SerializeField] private float _trueWayChance;
    [SerializeField] private float _fallingTime;
    
    private PlayerConfig _playerConfig;
    private BoostSpawner _boostSpawner;
    private List<Boost> _boostWay;
   
    
    private AnimationCurve _currentCurve;
    private float _segmentDuration;
    private float _expandedTime = 0;
    private Vector3 _initialPos;
    private Vector3 _targetPos;
    private int _countGetBoosts;


    public event Action EndFlight;
    
    [Inject]
    public void Init(BoostSpawner boostSpawner, PlayerConfig playerConfig) {
        _boostSpawner = boostSpawner;
        _playerConfig = playerConfig;
    }


    public void Enter() {
        Debug.Log("Enter");
        
        PlayerRotateLocalX(-25);
        TpBotNearPlayer();
        // Сбросить кол-во бустов надо
        ResetCountBoosts();
        _boostWay = _boostSpawner.GetRandomWay(_trueWayChance);
        if (_boostWay.Count == 0) {
            Debug.LogError("Цепочка бустов пустая!");
            return;
        }
        Debug.Log("Кол-во бустов у бота: " + _boostWay.Count);
        SetBooster(_boostWay[0].randomTrajectory, _boostWay[0].transform.position);
        BotFlightCycle();
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
    private async UniTask BotFlightCycle() {
        Debug.Log(_boostWay.Count);
        float currentY = 20f;
        
        while (_countGetBoosts <= _boostWay.Count || currentY > 0.2f) {
            FlightLogic();
            currentY = transform.position.y;
            await UniTask.WaitForFixedUpdate();
        }
        // Чутка подождать пока полежит
        PlayerRotateLocalX(-80);
        await UniTask.Delay(700);
        EndFlight?.Invoke();
    }
    
    public void Exit() {
        transform.position = _playerConfig.PlayerSpawnPosition;
    }
    
    

    private void TpBotNearPlayer() {
        Vector3 flightPosition = _flightPosition;
        flightPosition.x += Random.Range(-7, 7);
        transform.position = flightPosition;
    }

    private void ResetCountBoosts() {
        _countGetBoosts = 0;
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
        transform.localRotation = Quaternion.Euler(Vector3.zero);
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
        Debug.Log("Поворот игрока: " + transform.localRotation);
    }
    

}
