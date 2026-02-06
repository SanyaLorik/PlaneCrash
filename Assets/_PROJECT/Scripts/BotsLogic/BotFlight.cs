using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BotFlight : FlightObject, IBotBehaviour {
    [SerializeField] private int _countBoostEquilizeSpeed;
    [SerializeField] private float _botSpeedCorrect;
    [SerializeField] private Vector3 _flightPosition;
    [SerializeField] private Transform _botModelForRotate;
    [Range(0,1), SerializeField] private float _trueWayChance;
    [SerializeField] private float _fallingTime;
    
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Collider _collider;
    
    private List<Boost> _boostWay;
    private int _countGetBoosts;
    
    public event Action EndFlight;
    
    [Inject] private LevelBounds _levelBounds;
    [Inject] private PlayerConfig _playerConfig;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private BoostSpawner _boostSpawner;
    
    
    public void GoToFall() {
        // Логика падения
        _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
        BotFallAsync(_token).Forget();
    }

    
    public void SetBooster(AnimationCurve curve, Vector3 nextBoost) {
        _currentCurve = curve;
        _expandedTime = 0f;
        _initialPos = transform.position;
        TargetPos = nextBoost;
        float distance = Vector3.Distance(_initialPos, TargetPos);
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
    
    public void Exit() {
        _collider.enabled = false;
        TpToSpawn();
    }


    public void Enter() {
        _collider.enabled = true;
        _token =  UniTaskHelper.CreateNewToken(ref _tokenSource);
        RotateLocalXAsync(-25, _token).Forget();
        TpNearPlayer();
        // Сбросить кол-во бустов надо
        ResetCountBoosts();
        _boostWay = _boostSpawner.GetRandomWay(_trueWayChance);
        if (_boostWay.Count == 0) {
            Debug.LogError("Цепочка бустов пустая!");
            return;
        }
        SetBooster(_boostWay[0].randomTrajectory, _boostWay[0].transform.position);
        StartFlightCycle();
    }

    private void StartFlightCycle() {
        _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
        BotFlightCycleAsync(_token).Forget();
    }
    
    private async UniTaskVoid BotFlightCycleAsync(CancellationToken token) {
        float currentY = 20f;
        
        while (_countGetBoosts <= _boostWay.Count && currentY > 0.6f && !token.IsCancellationRequested) {
            FlightLogic();
            currentY = transform.position.y;
            await UniTask.WaitForFixedUpdate();
        }
        // Чутка подождать пока полежит
        await BotIsFalledAsync(token);
    }
    


    private async UniTaskVoid BotFallAsync(CancellationToken token) {
        _initialPos = transform.position;
        TargetPos = new Vector3(
            Random.Range(_levelBounds.LeftX, _levelBounds.RightX),
            0.3f,
            _initialPos.z + Random.Range(100f,300f)
        );
        float timeToFall = 5f;
        _expandedTime = 0f;
        while (_expandedTime < timeToFall) {
            float progress = _expandedTime / timeToFall;
            
            Vector3 newPos =  transform.position;
            newPos.x = Mathf.Lerp(_initialPos.x, TargetPos.x, progress);
            newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, progress);
            newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, progress);
            
            transform.position = newPos;
            
            _expandedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(token);
        }

        await BotIsFalledAsync(token);
    }


    private async UniTask BotIsFalledAsync(CancellationToken token) {
        RotateLocalXAsync(-80, token).Forget();
        await UniTask.Delay(2000, cancellationToken: token);
        EndFlight?.Invoke();
    } 


    private void TpNearPlayer() {
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
        newPos.y = Mathf.Lerp(_initialPos.y, TargetPos.y, normalizedTime) + height;
        newPos.z = Mathf.Lerp(_initialPos.z, TargetPos.z, normalizedTime);
        newPos.x = Mathf.Lerp(_initialPos.x, TargetPos.x, normalizedTime);
        _expandedTime += Time.fixedDeltaTime;

        transform.position = newPos;
    }
    
    
    private async UniTask RotateLocalXAsync(float TargetPosAngleX, CancellationToken token) {
        if (token.IsCancellationRequested) { return; } // Исправить выглядит как гавно
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        float duration = 1f;
    
        Vector3 TargetPosLocalEuler;
        TargetPosLocalEuler = new Vector3(TargetPosAngleX, 0f, 180f);
        
    
        Quaternion startRot = _botModelForRotate.localRotation;
        Quaternion targetPosRot = Quaternion.Euler(TargetPosLocalEuler);
    
        float elapsedTime = 0;
    
        while (elapsedTime < duration &&  !token.IsCancellationRequested) {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;
        
            _botModelForRotate.localRotation = Quaternion.Slerp(startRot, targetPosRot, t);
        
            await UniTask.Yield();
        }
    
        _botModelForRotate.localRotation = targetPosRot;
    }
    
    
    private void TpToSpawn() {
        transform.position = _levelBounds.PlayerSpawnPoint.position;
        _rb.linearVelocity = Vector3.zero;
        
    }


}
