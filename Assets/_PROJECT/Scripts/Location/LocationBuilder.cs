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
    

    private CancellationTokenSource _tokenSource;

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _nextSpawnDistance = (_playerMovement.Transform.position.z + _distanceToSpawnNew);
            _nextDestroyDistance =  (_playerMovement.Transform.position.z + _distanceToDestroyOld);
            _lastEnd = _firstPoint.position;
            _tokenSource = new CancellationTokenSource();
            ConstructRoutine(_tokenSource.Token).Forget();
        }
        else if(state == PlayerState.Walking) {
            _tokenSource?.Cancel();
            HideCreations();
        }

        Debug.Log(_localizationDataPC.lol);
    }


    private async UniTask ConstructRoutine(CancellationToken token) {
        // Первичный спавн обьектов
        int indexDeletedModule = 0;
        while (_lastEnd.z < _distanceToSpawn) {
            SpawnNext();
            await UniTask.Yield(token);
        }
        while (!token.IsCancellationRequested) {
            if (_playerMovement.Transform.position.z > _nextSpawnDistance) {
                _nextSpawnDistance += _distanceToSpawnNew;
                SpawnNext();
            }

            if (_playerMovement.Transform.position.z > _nextDestroyDistance) {
                _nextDestroyDistance += _distanceToDestroyOld;
                if (_createdModules.Count > 0) {
                    HideOldestModule();
                }
            }
            await UniTask.Yield(token);
        }

    }

    private void HideCreations() {
        Debug.Log("_createdModules.count : " + _createdModules.Count);
        foreach (var module in _createdModules) {
            _poolManager.ReturnObjectToPool(module.gameObject, PoolType.LocationObject);
            module.HideObjects();
        } 
        _createdModules.Clear();
    }

    private void HideOldestModule() {
        var module = _createdModules[0];

        module.HideObjects();
        _poolManager.ReturnObjectToPool(module.gameObject, PoolType.LocationObject);

        _createdModules.RemoveAt(0);
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
    }
    

}
