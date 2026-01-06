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
    [SerializeField] private Transform _cruiser;
    
    [Header("Граница спауна")]
    [SerializeField] private Boost _boostPrefab;
    [SerializeField] private Boost _falseBoostPrefab;

    [SerializeField] private PairedValue<float> _xZone;
    [SerializeField] private PairedValue<float> _yZone;
    [SerializeField] private PairedValue<float> _distanceZone;
    [SerializeField] private PairedValue<float> _falseWayLengthDivider;
    [SerializeField] private float _lastDistanceMin;
    
    
    
    public AnimationCurve[] _curves;
    public List<Boost> _boosts1;
    public List<Boost> _boosts2;
    public List<Boost> _boosts3;

    private PlayerMovement _playerMovement;
    
    
    private void Start() {
        _playerMovement = _player.GetComponent<PlayerMovement>();
        SpawnBoostWays(true,_player.transform.position, _cruiser.transform.position, _boosts1);
        SpawnBoostWays(true,_player.transform.position, _cruiser.transform.position, _boosts2);
        SpawnBoostWays(false,_player.transform.position, _cruiser.transform.position, _boosts3);
        SpawnBoostWays(false,_player.transform.position, _cruiser.transform.position, _boosts3);
        SpawnEntranceBoost();
    }


    private void SpawnEntranceBoost() {
        _playerMovement.SetBooster(_curves[0], _boosts1[0].transform.position); // действует на игрока сразу
    }
    

    
    
    private void SpawnBoostWays(bool trueChain, Vector3 playerPosition, Vector3 cruiserPosition, List<Boost> _boosts) {
    
        List<float> spawnPoints = new List<float>();
        float currentPosition = 0f;
        Vector3 endPosition = cruiserPosition;

        Boost boostPrefab = _boostPrefab;
        if (!trueChain) {
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
    
        // Точки спауна
        foreach (float zPos in spawnPoints) {
            Vector3 spawnPosition = new Vector3(
                Random.Range(_xZone.From,_xZone.To), 
                Random.Range(_yZone.From,_yZone.To),                  
                zPos                  
            );
        
            _boosts.Add(Instantiate(boostPrefab, spawnPosition, Quaternion.identity)); 
        }

        for (int i = 0; i < _boosts.Count; i++) {
            if (i != _boosts.Count - 1) {
                _boosts[i].nextBooster = _boosts[i + 1].transform.position;
                _boosts[i].randomTrajectory = _curves[Random.Range(0, _curves.Length)];
                // Debug.Log("Следующий буст в " + _boosts[i].nextBooster.z);
            }
            else {
                _boosts[i].nextBooster = endPosition;
                _boosts[i].randomTrajectory = _curves[1];
                Debug.Log("Конечный буст в  " + endPosition.z);
            }
        }
    }
    

}
