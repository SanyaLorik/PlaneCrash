using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    public PairedValue<int> EnemiesPerChunk;
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
    private BoostSpawner _boostsSpawner;
    private ZoneManager _zoneManager;
    private TrapPositionCalculator _trapPositionCalculator;
    
    
    private List<Boost> _boosts;
    private List<BombTrap> _traps = new();
    private LevelBounds _levelBounds;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, BoostSpawner boostSpawner, ZoneManager zoneManager, LevelBounds levelBounds) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostsSpawner = boostSpawner;
        
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
        GetAllBoost();
        SpawnAllTrapsAsync().Forget();

    }

    private async UniTask SpawnAllTrapsAsync() {
        // Начиная с зоны _zonesInfo[0] 
        await SpawnFakeTraps();
        // Дальше 1-3 зоны
        await SpawnMoveTraps(_zonesInfo[1]);
        await SpawnMoveTraps(_zonesInfo[2]);
        await SpawnMoveTraps(_zonesInfo[3]);
        await SpawnTrapsNearBoosts();
    } 

    private async UniTask SpawnFakeTraps() {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageEnd);
        Debug.Log($"Спавн фейк трапа будет в диапазоне: ({startZCoord}:{endZCoord})");

        foreach (var boost in _boosts) {
            if(Random.value > 0.6f) continue;
            if (boost.transform.position.z > endZCoord) continue;
            if (boost.hasTrap) continue;


            boost.hasTrap = true;
            Vector3 _trapPosition = _trapPositionCalculator.GetNearBoostPosition(boost.transform.position);
            BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
            Debug.Log("Спавн фейк трапа в :" + _trapPosition);
            _trap.transform.localPosition = _trapPosition;
            
            _traps.Add(_trap);
            await UniTask.WaitForEndOfFrame();
        }
    }
    
    
    
    private async UniTask SpawnMoveTraps(ZoneInfo zone) {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (zone.PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (zone.PercentageEnd);
        Debug.Log($"Спавн зоны {zone.ZoneName} будет в диапазоне: ({startZCoord}:{endZCoord})");

        int chunks = zone.ChunksCount;

        List<ChunkDistance> chunksDiapasone = GetChunksDistances(chunks, startZCoord, endZCoord);

        
        // В каждой зоне zone.EnemiesPerChunk ловушэк
        
        
        foreach (var diapasone in chunksDiapasone) {
            int enemiesCount = Random.Range(zone.EnemiesPerChunk.From, zone.EnemiesPerChunk.To);
            float distance = diapasone.Z2 - diapasone.Z1;
            // Debug.Log("Дистанция зоны: " + distance);
            // Debug.Log("Кол-во ловушек: " + enemiesCount);
            
            for (int i = 0; i < enemiesCount; i++) {
                float x = Random.Range(_levelBounds.LeftX, _levelBounds.RightX); // пока просто где-то
                float y = Random.Range(_levelBounds.MinimumY+5f, 40f); // пока просто где-то
                float z = diapasone.Z1 + (distance * (i) / enemiesCount);  
                
                Vector3 _trapPosition = new Vector3(x, y, z);
                // Debug.Log("Спавн в " + _trapPosition);
                    
                    
                BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
                _trap.Init(_levelBounds, _boostsSpawner);
                _trap.SetMovable();
                // Debug.Log("Спавн движущегося трапа в: " + _trapPosition);
                _trap.transform.localPosition = _trapPosition;
                
                _traps.Add(_trap);
                await UniTask.WaitForEndOfFrame();
            }
        }

    }
    
    
    private async UniTask SpawnTrapsNearBoosts() {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[1].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[^1].PercentageEnd);
        Debug.Log($"Спавн ловушек возле бустов будет в диапазоне: ({startZCoord}:{endZCoord})");

        
        foreach (var boost in _boosts) {
            if(Random.value < 0.1f) continue;  
            Vector3 _trapPosition = _trapPositionCalculator.GetInBoostPosition(boost.transform.position);
            
            BombTrap _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
            _trap.Init(_levelBounds, _boostsSpawner);
            _trap.SetMovable();
            
            // Debug.Log("Спавн движущегося трапа в: " + _trapPosition);
            _trap.transform.localPosition = _trapPosition;
            
            _traps.Add(_trap);
            await UniTask.WaitForEndOfFrame();
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
            Debug.Log($"{i+1}ый чанк, Дистанция: ({chunksDiapasone[i].Z1:F1} - {chunksDiapasone[i].Z2:F1})");
        }
        return chunksDiapasone;
    }


    private void ClearTraps() {
        foreach (var trap in _traps) {
            Destroy(trap.gameObject);
        }
        _traps.Clear();
    }
    

    private void GetAllBoost() {
        _boosts = _boostsSpawner.GetAllBoosts();
        // Debug.Log("Кол-во бустов: " + _boosts.Count);
    }

    
    private void CheckPercentCorrect() {
        // float sum = 0f;
        // foreach (var info in _zonesInfo) {
        //     sum += info.PercentageStart;
        // }
        // if (sum > 1f) {
        //     Debug.LogWarning("Сумма процентов > 100");
        // }
        // Debug.Log(sum);
    }
    
    
}
