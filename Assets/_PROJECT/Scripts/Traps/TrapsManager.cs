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

public class ChunkDistance {
    public float Z1;
    public float Z2;
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
    private LevelBounds _levelBounds;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, BoostSpawner boostSpawner, ZoneManager zoneManager, LevelBounds levelBounds) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostSpawner = boostSpawner;
        
        _zoneManager = zoneManager;
        _levelBounds = levelBounds;
    }

    private void Awake() {
        _trapPositionCalculator = GetComponent<TrapPositionCalculator>();
        CheckPercentCorrect();
    }

    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state != PlayerState.Flight) return;
        ClearTraps();
        
        // Нам нужны все бусты
        GetAllBoostPositions();
        // Начиная с зоны _zonesInfo[0] 
        SpawnFakeTraps();
        // Дальше 1-3 зоны
        SpawnMoveTraps(_zonesInfo[1]);

    }

    private void SpawnFakeTraps() {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageEnd);
        Debug.Log($"Спавн фейк трапа будет в диапазоне: ({startZCoord}:{endZCoord})");

        foreach (var boost in _boostPositions) {
            if(Random.value > 0.6f) continue;
            if (boost.z > endZCoord) continue;
            
            Vector3 _trapPosition = _trapPositionCalculator.GetNearBoostPosition(boost);
            BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
            
            Debug.Log("Спавн фейк трапа в :" + _trapPosition);
            _trap.transform.localPosition = _trapPosition;
            
            _traps.Add(_trap);
        }
    }
    
    
    
    private void SpawnMoveTraps(ZoneInfo zone) {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (zone.PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (zone.PercentageEnd);
        Debug.Log($"Спавн зоны {zone.ZoneName} будет в диапазоне: ({startZCoord}:{endZCoord})");

        int chunks = zone.ChunksCount;

        List<ChunkDistance> chunksDiapasone = GetChunksDistances(chunks, startZCoord, endZCoord);

        
        // В каждой зоне zone.EnemiesPerChunk ловушэк
        
        foreach (var diapasone in chunksDiapasone) {
            foreach (var boost in _boostPositions) {
                if(Random.value > 0.65f) continue;
                if (boost.z < diapasone.Z1) continue;
                if (boost.z > diapasone.Z2) continue;
                
                Vector3 _trapPosition = _trapPositionCalculator.GetInBoostPosition(boost);
                BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
                _trap.Init(_levelBounds);
                _trap.SetMovable();
                // Debug.Log("Спавн движущегося трапа в: " + _trapPosition);
                _trap.transform.localPosition = _trapPosition;
                
                _traps.Add(_trap);
            }
        }

    }

    private List<ChunkDistance> GetChunksDistances(int chunks, float startZCoord, float endZCoord) {
        List<ChunkDistance> chunksDiapasone = new ();
        float distance = endZCoord-startZCoord;

        for (int i = 0; i < chunks; i++) {
            ChunkDistance chunkDistance = new ChunkDistance{
                Z1 = startZCoord + distance * i/chunks,
                Z2 = startZCoord + distance * (i+1)/chunks
            };
            
            chunksDiapasone.Add(chunkDistance);
            Debug.Log($"{i+1}ый чанк, Дистанция: ({chunksDiapasone[i].Z1}:{chunksDiapasone[i].Z2})");
        }
        return chunksDiapasone;
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
