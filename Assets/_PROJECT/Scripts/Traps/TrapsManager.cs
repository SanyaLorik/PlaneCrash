using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using Unity.Collections;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public struct ZoneInfo {
    public string ZoneName;
    [Range(0,1), SerializeField] public float PercentageStart;
    [Range(0,1), SerializeField] public float PercentageEnd;
    public int ChunksCount;
    public PairedValue EnemiesPerChunk;
}


public class TrapsManager : MonoBehaviour {
    // Наверное зоны начиная с 3
    [SerializeField] private List<ZoneInfo> _zonesInfo;
    
    // Лучше потом прокидывать через зенжу
    [SerializeField] private BombTrap _trapObject;

    
    
    private PlayerStateManager _playerStateManager;
    private BoostSpawner _boostSpawner;
    private ZoneManager _zoneManager;
    private TrapPositionCalculator _trapPositionCalculator;
    
    
    private List<Vector3> _boostPositions;
    private List<BombTrap> _traps = new();
    
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, BoostSpawner boostSpawner, ZoneManager zoneManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostSpawner = boostSpawner;
        
        _zoneManager = zoneManager;
    }

    private void Awake() {
        _trapPositionCalculator = GetComponent<TrapPositionCalculator>();
        CheckPercentCorrect();
    }

    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state != PlayerState.Flight) return;
        // Нам нужны все бусты
        GetAllBoostPositions();
        // Начиная с зоны _zonesInfo[0] 
        SpawnFakeTraps();
        // Дальше 1-3 зоны
        SpawnMoveTraps(1);

    }

    private void SpawnFakeTraps() {
        ClearTraps();
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageEnd);
        Debug.Log($"Спавн фейк трапа будет в диапазоне: ({startZCoord}:{endZCoord})");

        foreach (var boost in _boostPositions) {
            if(Random.value > 0.5f) continue;
            if (boost.z > endZCoord) continue;
            
            Vector3 _trapPosition = _trapPositionCalculator.GetNearBoostPosition(boost);
            BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
            
            Debug.Log("Спавн фейк трапа в :" + _trapPosition);
            _trap.transform.localPosition = _trapPosition;
            
            _traps.Add(_trap);
        }
    }
    
    
    
    private void SpawnMoveTraps(int zoneNumber) {
        ClearTraps();
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[zoneNumber].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[zoneNumber].PercentageEnd);
        Debug.Log($"Спавн зоны {zoneNumber} трапа будет в диапазоне: ({startZCoord}:{endZCoord})");
        
        // теперь разбиваем на чанки, но пока похуй
        

        foreach (var boost in _boostPositions) {
            if(Random.value > 0.5f) continue;
            if (boost.z > endZCoord) continue;
            if (boost.z < startZCoord) continue;
            
            Vector3 _trapPosition = _trapPositionCalculator.GetInBoostPosition(boost);
            BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
            _trap.SetMovable();
            Debug.Log("Спавн движущегося трапа в: " + _trapPosition);
            _trap.transform.localPosition = _trapPosition;
            
            _traps.Add(_trap);
        }
    }

    
    
    
    
    private void ClearTraps() {
        foreach (var trap in _traps) {
            Destroy(trap.gameObject);
        }
        _traps.Clear();
    }
    

    private void GetAllBoostPositions() {
        _boostPositions = _boostSpawner.GetAllBoosts();
        Debug.Log("Кол-во бустов: " + _boostPositions.Count);
    }

    
    private void CheckPercentCorrect() {
        float sum = 0f;
        foreach (var info in _zonesInfo) {
            sum += info.PercentageStart;
        }
        if (sum > 1f) {
            Debug.Log("Сумма процентов > 100");
        }
        Debug.Log(sum);
    }
}
