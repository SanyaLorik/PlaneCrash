using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
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
    
    private PlayerStateManager _playerStateManager;
    
     
    private List<LocationModule> _createdModulesActive = new ();
    private List<LocationModule> _createdModulesResetBuffer = new ();
    
    private CancellationTokenSource _tokenSource;
    
    
    
    [Inject] private ObjectPoolManager _poolManager;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private DiContainer _diContainer;
    [Inject] LocalizationDataPC _localizationDataPC;
    
    
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void Awake() {
        StartInitAsync().Forget();
    }


    private async UniTask StartInitAsync() {
        _deletePoint = _distanceToSpawn;
        _lastEnd = _firstPoint.position;
        
        _tokenSource = new CancellationTokenSource();
        await SpawnStartDistanceAsync(_tokenSource.Token);
        
        _createdModulesActive.AddRange(_createdModulesResetBuffer);
        _createdModulesResetBuffer.Clear();
    }


    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            _nextSpawnDistance = (_playerMovement.Transform.position.z + _distanceToSpawnNew);
            _nextDestroyDistance =  _distanceToSpawn;
            
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            ConstructRoutine(_tokenSource.Token).Forget();
        }
        else if (state == PlayerState.Grounded || state == PlayerState.Cruisered) {
            // Чуть чуть сзади именно что
            _tokenSource?.Cancel();
            // За спиной игрока все удаляем
            
            _deletePoint = Mathf.Max(_playerMovement.Transform.position.z, _distanceToSpawn);
            // игрок перелетел спавн
            if (!Mathf.Approximately(_deletePoint, _distanceToSpawn)) {
                RespawnStartingLocation();
            }
        }
        else if (state == PlayerState.Walking && !Mathf.Approximately(_deletePoint, _distanceToSpawn)) {
            HideCreations();
            _createdModulesActive.AddRange(_createdModulesResetBuffer);
            _createdModulesResetBuffer.Clear();
        }

    }

    private void RespawnStartingLocation() {
        _lastEnd = _firstPoint.position;
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
            
            if (playerZ > _nextSpawnDistance && !token.IsCancellationRequested) {
                _nextSpawnDistance += _distanceToSpawnNew;
                SpawnNext(_createdModulesActive);
            }

            if (playerZ > _nextDestroyDistance) {
                _nextDestroyDistance += _distanceToDestroyOld;
                if (_createdModulesActive.Count > 0 && !token.IsCancellationRequested) {
                    HideOldestModule();
                }
            }
            await UniTask.Yield(token);
        }
    }

    private async UniTask SpawnStartDistanceAsync(CancellationToken token) {
        Debug.LogWarning("SpawnStartDistanceAsync");
        while (_lastEnd.z < _distanceToSpawn && !token.IsCancellationRequested) {
            SpawnNext(_createdModulesResetBuffer);
            await UniTask.Yield(token);
        }
    }

    private float _deletePoint;
    

    
    private void HideCreations() {
        Debug.LogWarning("HideCreationsAfterPlayerFall");

        for (int i = _createdModulesActive.Count - 1; i >= 0; i--) {
            var module = _createdModulesActive[i];
            module.HideObjects();
            _poolManager.ReturnObjectToPool(module.gameObject, PoolType.LocationObject);

            _createdModulesActive.RemoveAt(i);
        }
    }


    private void HideOldestModule() {
        Debug.Log("HideOldestModule");
        LocationModule oldestModule = _createdModulesActive[0];
        foreach (var createdModule in _createdModulesActive) {
            if (createdModule.transform.position.z < oldestModule.transform.position.z) {
                oldestModule = createdModule;
            }
        }

        oldestModule.HideObjects();
        _poolManager.ReturnObjectToPool(oldestModule.gameObject, PoolType.LocationObject);
        _createdModulesActive.Remove(oldestModule);
    }
    
    
   
    private void SpawnNext(List<LocationModule> list) {
        var prefab = _buildingModulesPrefabs[Random.Range(0, _buildingModulesPrefabs.Count)];
        
        LocationModule module = _poolManager.Spawn<LocationModule>(prefab.gameObject, Vector3.zero, PoolType.LocationObject);
        list.Add(module);
        module.Init(_diContainer);
        
        // Offset
        Vector3 offset = module.transform.position - module.Start.position;
        module.transform.position = _lastEnd + offset;
        
        _lastEnd = module.End.position;
        module.GenerateProps();
        // Debug.Log("Создани модуля в " + module.transform.position.z);
    }
    

}
