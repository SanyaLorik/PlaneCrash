using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerMagnet : MonoBehaviour {
    [SerializeField] private float _baseMagnet = 10f;
    [SerializeField] private Transform _playerTransform;
    
    private readonly List<IMagnetic> _magneticsBoost = new();
    private readonly List<IMagnetic> _magneticsMoney = new();
    
    private IMagnetic _currentTargetBoost;
    private IMagnetic _currentTargetMoney;
    
    private PlayerStateManager _playerStateManager; 
    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;
    private IPlayerStatsReadOnly _playerStats;
    
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, IPlayerStatsReadOnly playerStats) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _playerStats = playerStats;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        _magneticsBoost.Clear();
        _magneticsMoney.Clear();
        if (state == PlayerState.Flight) {
            _token = UniTaskHelper.CreateNewToken(ref _tokenSource);
            MonitoringTargets(_token).Forget();
        }
        else {
            _tokenSource?.Cancel();
        }
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out IMagnetic magnetic)) {
            Debug.Log("Попадание в коллайдер " + magnetic.Type);
            if (magnetic.Type == MagneticType.Boost) {
                _magneticsBoost.Add(magnetic);
                return;
            }
            _magneticsMoney.Add(magnetic);
            
        }
    }
    
    private void OnTriggerExit(Collider collider) {
        if (collider.TryGetComponent(out IMagnetic magnetic)) {
            Debug.Log("Попадание в коллайдер " + magnetic);
            if (magnetic.Type == MagneticType.Boost) {
                _magneticsBoost.Add(magnetic);
                return;
            }
            _magneticsMoney.Add(magnetic);
        }
    }



    private async UniTask MonitoringTargets(CancellationToken token) {
        while (!_token.IsCancellationRequested) {
            // Буст
            _currentTargetBoost = GetClosest(_magneticsBoost);
            foreach (var obj in _magneticsBoost) {
                if (obj == _currentTargetBoost && obj.CanBeMagnetic) {
                    obj.Attract(_playerTransform.position, _baseMagnet * _playerStats.MagnetSpeed);
                } 
            }
        
            // Бабка
            _currentTargetMoney = GetClosest(_magneticsMoney);
            foreach (var obj in _magneticsMoney) {
                if (obj == _currentTargetMoney && obj.CanBeMagnetic) {
                    obj.Attract(_playerTransform.position, _baseMagnet * _playerStats.MagnetSpeed);
                } 
            }
            await UniTask.Yield(token);
        }
    }


    private IMagnetic GetClosest(List<IMagnetic> magnetics) {
        float minDist = float.MaxValue;
        IMagnetic closest = null;

        foreach (var obj in magnetics) {
            float d = Vector3.SqrMagnitude(obj.Position - transform.position);

            if (d < minDist) {
                minDist = d;
                closest = obj;
            }
        }

        return closest;
    }

    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

}
