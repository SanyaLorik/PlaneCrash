using System;
using System.Collections.Generic;
using System.Linq;
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



[Serializable]
public enum TrapRole {
    OnPath,     // прямо на пути
    Side,       // сбоку
    Fake,       // пугает
}


[Serializable]
public struct TrapRoleEntry {
    public TrapRole Role;
    public List<TrapController> Traps;
}




public class ChunkDistance {
    public float Z1;
    public float Z2;
}


public class TrapsManager : MonoBehaviour {
    // Наверное зоны начиная с 3
    [SerializeField] private List<ZoneInfo> _zonesInfo;
    [SerializeField] private List<TrapRoleEntry> _trapsByRoleList;
    
    
    private Dictionary<TrapRole, List<TrapController>> _trapsDictionary;
    private List<TrapController>  _сreatedTraps = new ();
    
    
    
    private List<Boost> _boosts;
    private PlayerStateManager _playerStateManager;
    
    [Inject] private DiContainer _container;
    
    [Inject] private LevelBounds _levelBounds;
    [Inject] private BoostSpawner _boostsSpawner;
    [Inject] private ZoneManager _zoneManager;
    [Inject] private TrapPositionCalculator _trapPositionCalculator;
    
    [Inject]
    private void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void Awake() {
        _trapPositionCalculator = GetComponent<TrapPositionCalculator>();
        CheckPercentCorrect();
        SerializeDict();
    }


    private void SerializeDict() {
        _trapsDictionary = _trapsByRoleList.ToDictionary(x => x.Role, x => x.Traps);
    }

    private TrapController GetRandomTrapForRole(TrapRole trapRole) {
        _trapsDictionary.TryGetValue(trapRole , out var trapsInDict);
        if (trapsInDict == null) {
            Debug.Log("Такого типа трапа нема: " + trapRole);
            return null;
        }
        return trapsInDict[Random.Range(0, trapsInDict.Count)];
    }
    
    
    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state != PlayerState.Flight) return;
        ClearTraps();
        
        // Нам нужны все бусты
        GetAllBoost();
        SpawnAllTrapsAsync().Forget();

    }

    private async UniTask SpawnAllTrapsAsync() {
        
        // Мне нужна ловушка типа X
        
        // Начиная с зоны _zonesInfo[0] 
        await SpawnFakeTraps();
        // Дальше 1-3 зоны
        await SpawnMoveTraps(_zonesInfo[1]);
        await SpawnMoveTraps(_zonesInfo[2]);
        await SpawnMoveTraps(_zonesInfo[3]);
        // await SpawnTrapsNearBoosts();
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
            if (_zoneManager.CurrentMultiplyer == 2f) {
                enemiesCount /= 2;
            }
            
            float distance = diapasone.Z2 - diapasone.Z1;
            // Debug.Log("Дистанция зоны: " + distance);
            // Debug.Log("Кол-во ловушек: " + enemiesCount);
            
            for (int i = 0; i < enemiesCount; i++) {
                TrapController trap = GetRandomMovableTrap();
                
                
                float x = GetXCoord(trap);
                float y = Random.Range(_levelBounds.MinimumY+5f, 60f); // пока просто где-то
                float z = GetChunkZCoord(diapasone, distance, i, enemiesCount);  

                Vector3 _trapPosition = new Vector3(x, y, z);
                
                
                TrapController _trap = Instantiate(trap, _trapPosition, Quaternion.identity);
                _trap.Init(_boostsSpawner, _levelBounds);
                _trap.transform.localPosition = _trapPosition;
                _trap.StartMoveTrap();
                
                
                _сreatedTraps.Add(_trap);
                await UniTask.WaitForEndOfFrame();
            }
        }

    }
    
    
    private async UniTask SpawnFakeTraps() {
        // _trapObject
        float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageStart);
        float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[0].PercentageEnd);
        Debug.Log($"Спавн фейк трапа будет в диапазоне: ({startZCoord}:{endZCoord})");

        foreach (var boost in _boosts) {
            if(Random.value > 0.8f) continue;
            if (boost.transform.position.z > endZCoord) continue;
            if (boost.hasTrap) continue;


            boost.hasTrap = true;
            Vector3 _trapPosition = _trapPositionCalculator.GetNearBoostPosition(boost.transform.position);
            TrapController _trap = Instantiate(GetRandomTrapForRole(TrapRole.Fake), _trapPosition, Quaternion.identity);
            _trap.Init(_boostsSpawner, _levelBounds);
            _trap.transform.localPosition = _trapPosition;
            
            _trap.StartMoveTrap();
            _сreatedTraps.Add(_trap);
            await UniTask.WaitForEndOfFrame();
        }
    }

    private float GetXCoord(TrapController trap) {
        if (trap.TrapRole == TrapRole.OnPath) {
            return Random.Range(_levelBounds.LeftX, _levelBounds.RightX);
        }
        // Слева или справя
        return Random.value > 0.5f ? _levelBounds.LeftX : _levelBounds.RightX;
    }

    private float GetChunkZCoord(ChunkDistance diapasone, float distance, int i, int enemiesCount) {
        float z = diapasone.Z1 + (distance * (i) / enemiesCount);
        return z;
    }

    private TrapController GetRandomMovableTrap() {
        TrapRole role = Random.value > 0.5 ? TrapRole.Side : TrapRole.OnPath;
        TrapController trap = GetRandomTrapForRole(role);
        trap.TrapRole = role;
        
        return trap;
    }


    
    
    // private async UniTask SpawnTrapsNearBoosts() {
    //     // _trapObject
    //     float startZCoord = _zoneManager.CruiserDistance * (_zonesInfo[1].PercentageStart);
    //     float endZCoord = _zoneManager.CruiserDistance * (_zonesInfo[^1].PercentageEnd);
    //     Debug.Log($"Спавн ловушек возле бустов будет в диапазоне: ({startZCoord}:{endZCoord})");
    //
    //     
    //     foreach (var boost in _boosts) {
    //         if(Random.value < 0.1f) continue;  
    //         Vector3 _trapPosition = _trapPositionCalculator.GetInBoostPosition(boost.transform.position);
    //         
    //         TrapController _trap = Instantiate(_trapObject, _trapPosition, Quaternion.identity);
    //         _trap.Init(_boostsSpawner, _levelBounds);
    //         
    //         
    //         
    //         // Debug.Log("Спавн движущегося трапа в: " + _trapPosition);
    //         _trap.transform.localPosition = _trapPosition;
    //         
    //         _сreatedTraps.Add(_trap);
    //         await UniTask.WaitForEndOfFrame();
    //     }
    //
    // }

    private List<ChunkDistance> GetChunksDistances(int chunks, float startZCoord, float endZCoord) {
        List<ChunkDistance> chunksDiapasone = new ();
        float distance = endZCoord-startZCoord;

        for (int i = 0; i < chunks; i++) {
            ChunkDistance chunkDistance = new ChunkDistance{
                Z1 = startZCoord + distance * i/chunks,
                Z2 = startZCoord + distance * (i+1)/chunks
            };
            
            chunksDiapasone.Add(chunkDistance);
            // Debug.Log($"{i+1}ый чанк, Дистанция: ({chunksDiapasone[i].Z1:F1} - {chunksDiapasone[i].Z2:F1})");
        }
        return chunksDiapasone;
    }


    private void ClearTraps() {
        foreach (var trap in _сreatedTraps) {
            Destroy(trap.gameObject);
        }
        _сreatedTraps.Clear();
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
