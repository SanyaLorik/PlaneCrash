using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

[Serializable]
public struct ZoneInfo {
    [Range(0,1), SerializeField] public float Percentage;
    public int ChunksCount;
    public PairedValue EnemiesPerChunk;
}


public class TrapsManager : MonoBehaviour {
    // Наверное зоны начиная с 3
    [SerializeField] private List<ZoneInfo> _zonesInfo;
    [SerializeField] private float _fakeZoneTrapsCount;

    private PlayerStateManager _playerStateManager;
    private BoostSpawner _boostSpawner;
    private List<Vector3> _boostPositions;
    
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager, BoostSpawner boostSpawner) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
        _boostSpawner = boostSpawner;
    }
    

    private void Start() {
        CheckPercentCorrect();
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state != PlayerState.Flight) return;
        // Нам нужны все бусты
        GetAllBoostPositions();
        // Начиная с зоны _zonesInfo[0] 
        
        
    }


    private void GetAllBoostPositions() {
        _boostPositions = _boostSpawner.GetAllBoosts();
        Debug.Log("Кол-во бустов: " + _boostPositions.Count);
    }

    
    
    private void CheckPercentCorrect() {
        float sum = 0f;
        foreach (var info in _zonesInfo) {
            sum += info.Percentage;
        }
        if (sum > 1f) {
            Debug.Log("Сумма процентов > 100");
        }
        Debug.Log(sum);
    }
}
