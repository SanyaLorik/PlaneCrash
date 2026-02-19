using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using Zenject;


public class LocationBuilder : MonoBehaviour {
    [SerializeField] private List<LocationModule>  _buildingModulesPrefabs;
    [SerializeField] private float _distanceToSpawn;
    // Надо чтоб оно было длинне самого мелкого буста а то будет прям видно как он исчезает
    [SerializeField] private float _distanceToSpawnNew;
    [SerializeField] private float _distanceToDestroyOld;
    [SerializeField] private Transform _firstPoint;

    
    // Указатель на последний в цепочке
    private Vector3 _lastEnd;
    private float _nextSpawnDistance;
    private float _nextDestroyDistance;
    
    
    
    [Inject] private ObjectPoolManager _poolManager;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private DiContainer _diContainer;

    private PlayerStateManager _playerStateManager;

    [Inject] LocalizationDataPC _localizationDataPC;
    
    
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void Awake() {
        _deletePoint = _distanceToSpawn; // берем в 2 раза больше просто чтоб при первом спавне не исчезало
        _lastEnd = _firstPoint.position;
        RespawnStartingLocation();
    }


    private CancellationTokenSource _tokenSource;

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _nextSpawnDistance = (_playerMovement.Transform.position.z + _distanceToSpawnNew);
            _nextDestroyDistance =  _distanceToSpawn;
            _tokenSource = new CancellationTokenSource();
            ConstructRoutine(_tokenSource.Token).Forget();
        }
        else if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
            // Чуть чуть сзади именно что
            _tokenSource?.Cancel();
            // За спиной игрока все удаляем
            
            // - _distanceToDestroyOld добавил проверим чтоб не делитилось слишком близко
            _deletePoint = Mathf.Max(_playerMovement.Transform.position.z-_distanceToDestroyOld, _distanceToSpawn);
            if (!Mathf.Approximately(_deletePoint, _distanceToSpawn)) {
                HideCreationsBeforePlayerFall();
                _lastEnd = _firstPoint.position;
                // Спавн на спавненском
                RespawnStartingLocation();
            }
        }
        else if (state == PlayerState.Walking && !Mathf.Approximately(_deletePoint, _distanceToSpawn)) {
            HideCreationsAfterPlayerFall();
        }

    }

    private void RespawnStartingLocation() {
        _tokenSource?.Cancel();
        _tokenSource = new CancellationTokenSource();
        SpawnStartDistanceAsync(_tokenSource.Token).Forget();
    }


    private async UniTask ConstructRoutine(CancellationToken token) {
        // Первичный спавн обьектов
        while (!token.IsCancellationRequested) {
            if (_playerMovement == null || _playerMovement.Transform == null)
                return;
            float playerZ = _playerMovement.Transform.position.z;
            
            if (playerZ > _nextSpawnDistance) {
                _nextSpawnDistance += _distanceToSpawnNew;
                SpawnNext();
            }

            if (playerZ > _nextDestroyDistance) {
                _nextDestroyDistance += _distanceToDestroyOld;
                if (_createdModules.Count > 0) {
                    HideOldestModule();
                }
            }
            await UniTask.Yield(token);
        }
    }

    private async UniTask SpawnStartDistanceAsync(CancellationToken token) {
        while (_lastEnd.z < _distanceToSpawn) {
            SpawnNext();
            await UniTask.Yield(token);
        }
    }

    private float _deletePoint;
    private void HideCreationsBeforePlayerFall() {
        // Debug.Log($"Удаляем все штуки ДО {_deletePoint} их {_createdModules.Count} шт");

        for (int i = _createdModules.Count - 1; i >= 0; i--) {
            var module = _createdModules[i];

            if (module.End.position.z < _deletePoint) {
                // Debug.Log("module.End.position.z = " + module.End.position.z);

                module.HideObjects();
                _poolManager.ReturnObjectToPool(module.gameObject, PoolType.LocationObject);

                _createdModules.RemoveAt(i);
            }
        }
    }

    
    private void HideCreationsAfterPlayerFall() {
        // Debug.Log($"Удаляем все штуки после {_deletePoint}");

        for (int i = _createdModules.Count - 1; i >= 0; i--) {
            var module = _createdModules[i];

            if (module.End.position.z >= _deletePoint) {
                module.HideObjects();
                _poolManager.ReturnObjectToPool(module.gameObject, PoolType.LocationObject);

                _createdModules.RemoveAt(i);
            }
        }
    }


    private void HideOldestModule() {
        LocationModule oldestModule = _createdModules[0];
        foreach (var createdModule in _createdModules) {
            if (createdModule.transform.position.z < oldestModule.transform.position.z) {
                oldestModule = createdModule;
            }
        }

        oldestModule.HideObjects();
        _poolManager.ReturnObjectToPool(oldestModule.gameObject, PoolType.LocationObject);
        _createdModules.Remove(oldestModule);
    }
    
    
    
    private List<LocationModule> _createdModules = new ();
    private void SpawnNext() {
        var prefab = _buildingModulesPrefabs[Random.Range(0, _buildingModulesPrefabs.Count)];
        
        LocationModule module = _poolManager.Spawn<LocationModule>(prefab.gameObject, Vector3.zero, PoolType.LocationObject);
        _createdModules.Add(module);
        module.Init(_diContainer);
        
        // Offset
        Vector3 offset = module.transform.position - module.Start.position;
        module.transform.position = _lastEnd + offset;
        
        _lastEnd = module.End.position;
        module.GenerateProps();
        // Debug.Log("Создани модуля в " + module.transform.position.z);
    }
    

}
