using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerMagnet : MonoBehaviour {
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private int _levelCountsToFullScale;
    
    private readonly List<IMagnetic> _magneticsBoost = new();
    private readonly List<IMagnetic> _magneticsMoney = new();
    
    private IMagnetic _currentTargetBoost;
    private IMagnetic _currentTargetMoney;
    
    private CancellationTokenSource _tokenSource;
    
    [SerializeField] private BoxCollider _collider;
    
    private PlayerStateManager _playerStateManager; 
    private IPlayerStatsReadOnly _playerStats;

    private UpgradesCalculator _upgradesCalculator;
    [Inject] private LevelBounds _levelBounds;
    [Inject] private BoostSpawner _boostSpawner;

    private int _magnetLevel = -1;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, IPlayerStatsReadOnly playerStats, UpgradesCalculator upgradesCalculator) {
        _upgradesCalculator = upgradesCalculator;
        _playerStateManager = playerStateManager;
        _playerStats = playerStats;
    }


    private Vector3 _maxColliderSize;
    private Vector3 _defaultColliderSize;


    private void OnEnable() {
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _playerStats.ChangeStats += PlayerStatsOnChangeStats;
    }

    private void Start() {
        CalculateColliderMaxScale();
        SetColiderInHead();
        PlayerStatsOnChangeStats();
        _defaultColliderSize  = _collider.size;
        
    }


    private void PlayerStatsOnChangeStats() {
        if (_magnetLevel != _playerStats.MagnetLevel) {
            // Перерасчет если изменился + 
            _magnetLevel = _playerStats.MagnetLevel;
        
            _collider.size = _upgradesCalculator.GetMagnetSizeByLevel(_defaultColliderSize, _maxColliderSize);
            Debug.Log("Collider Size = " + _collider.size);
            SetColiderInHead();
        }
    }


    private void CalculateColliderMaxScale() {
        float width = _levelBounds.CalculateFlightWidth() * 4;
        float height = _levelBounds.CalculateFlightHeight() * 4;
        float length = _boostSpawner.BoostDistance.To / 2;
        _maxColliderSize = new Vector3(width, height, length);
        // Debug.Log("_maxColliderSize" + _maxColliderSize);
    }


    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        _magneticsBoost.Clear();
        _magneticsMoney.Clear();
        if (state == PlayerState.Flight) {
            _collider.enabled = true;
            _tokenSource = new CancellationTokenSource();
        
            MonitoringTargets(_tokenSource.Token).Forget();
        }
        else {
            _collider.enabled = false;
            _tokenSource?.Cancel();
        }
    }

    
    
        

    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out IMagnetic magnetic)) {
            if (magnetic.Type == MagneticType.Boost) {
                _magneticsBoost.Add(magnetic);
            }
            else {
                _magneticsMoney.Add(magnetic);
            }
            
        }
    }
    
    private void OnTriggerExit(Collider collider) {
        if (collider.TryGetComponent(out IMagnetic magnetic)) {
            if (magnetic.Type == MagneticType.Boost) {
                _magneticsBoost.Remove(magnetic);
            }
            else {
                _magneticsMoney.Remove(magnetic);
            }
        }
    }



    private async UniTask MonitoringTargets(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            // Буст
            _currentTargetBoost = GetClosest(_magneticsBoost);
            foreach (var obj in _magneticsBoost) {
                if (obj == _currentTargetBoost && obj.CanBeMagnetic) {
                    obj.Attract(_playerTransform.position, _upgradesCalculator.GetMagnetKByLevel());
                } 
            }
        
            // Бабка
            _currentTargetMoney = GetClosest(_magneticsMoney);
            foreach (var obj in _magneticsMoney) {
                if (obj == _currentTargetMoney && obj.CanBeMagnetic) {
                    obj.Attract(_playerTransform.position, _upgradesCalculator.GetMagnetKByLevel());
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
    
    
    private void SetColiderInHead() {
        // Чтоб в попку не смотрел а вперед тольки
        Vector3 colliderPos = _collider.center;
        colliderPos.z = _collider.size.z / 2f;
        _collider.center = colliderPos;
    }

    private void OnDestroy() {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

}
