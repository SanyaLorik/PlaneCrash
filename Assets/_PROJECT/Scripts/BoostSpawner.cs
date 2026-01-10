using System;
using System.Collections;
using System.Collections.Generic;
using SanyaBeerExtension;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;




public class BoostSpawner : MonoBehaviour {
    [SerializeField] private Transform _player;
    
    [Header("Граница спауна")]
    [SerializeField] private Boost _boostPrefab;
    [SerializeField] private Boost _falseBoostPrefab;

    [SerializeField] private PairedValue<float> _xZone;
    [SerializeField] private PairedValue<float> _yZone;
    [SerializeField] private PairedValue<float> _distanceZone;
    [SerializeField] private PairedValue<float> _falseWayLengthDivider;
    [SerializeField] private float _lastDistanceMin;
    [SerializeField] private AnimationCurve[] _curves;

    private List<List<Boost>> _rightWays = new();
    private List<List<Boost>> _falseWays = new();
    

    private PlayerMovement _playerMovement;

    private void Start() {
        _playerMovement = _player.GetComponent<PlayerMovement>();
    }


    public void SpawnBoosts(Vector3 cruiserPosition) {
        ClearAllBoosts();
        
        _rightWays.Add(SpawnBoostWays(true, _player.transform.position, cruiserPosition));
        _rightWays.Add(SpawnBoostWays(true, _player.transform.position, cruiserPosition));
        _falseWays.Add(SpawnBoostWays(false, _player.transform.position, cruiserPosition));
        _falseWays.Add(SpawnBoostWays(false, _player.transform.position, cruiserPosition));
        Debug.Log(_rightWays[0].Count);
        Debug.Log(_rightWays[1].Count);
        Debug.Log(_falseWays[0].Count);
        Debug.Log(_falseWays[1].Count);
        SpawnEntranceBoost();
    }


    private void SpawnEntranceBoost() {
        _playerMovement.SetBooster(_curves[0], _rightWays[0][0].transform.position); // действует на игрока первым
    }

    private void ClearAllBoosts() {
        foreach (var chain in _rightWays) {
            foreach (var item in chain) {
                Destroy(item.gameObject);
            }
        }
        foreach (var chain in _falseWays) {
            foreach (var item in chain) {
                Destroy(item.gameObject);
            }
        }
        _rightWays.Clear();
        _falseWays.Clear();
    }

    
    
    private List<Boost> SpawnBoostWays(bool trueWay, Vector3 playerPosition, Vector3 cruiserPosition) {
    
        List<float> spawnPoints = new List<float>();
        float currentPosition = 0f;
        Vector3 endPosition = cruiserPosition;

        Boost boostPrefab = _boostPrefab;
        if (!trueWay) {
            boostPrefab = _falseBoostPrefab;
            endPosition /= Random.Range(_falseWayLengthDivider.From, _falseWayLengthDivider.To);
        }
        
        while (currentPosition < endPosition.z) {
            float newSpawnPoint = Random.Range(_distanceZone.From, _distanceZone.To);
            currentPosition += newSpawnPoint;
            if (endPosition.z - currentPosition > newSpawnPoint) {
                spawnPoints.Add(currentPosition);
            }
        }
        
    
        // Сортируем для гарантии порядка
        spawnPoints.Sort();
        List<Boost> boost = new ();
        
        // Точки спауна
        foreach (float zPos in spawnPoints) {
            Vector3 spawnPosition = new Vector3(
                Random.Range(_xZone.From,_xZone.To), 
                Random.Range(_yZone.From,_yZone.To),                  
                zPos                  
            );
        
            boost.Add(Instantiate(boostPrefab, spawnPosition, Quaternion.identity)); 
        }
        for (int i = 0; i < boost.Count; i++) {
            if (i != boost.Count - 1) {
                boost[i].nextBooster = boost[i + 1].transform.position;
                boost[i].randomTrajectory = _curves[Random.Range(0, _curves.Length)];
                // Debug.Log("Следующий буст в " + boost[i].nextBooster.z);
            }
            else {
                boost[i].nextBooster = endPosition;
                boost[i].randomTrajectory = _curves[1];
                // Debug.Log("Конечный буст в  " + endPosition.z);
            }
        }
        return boost;
    }
    

}
