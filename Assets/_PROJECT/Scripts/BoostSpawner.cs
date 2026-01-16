using System;
using System.Collections;
using System.Collections.Generic;
using SanyaBeerExtension;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UIElements;
using Zenject;
using Random = UnityEngine.Random;




public class BoostSpawner : MonoBehaviour {
    
    [Header("Граница спауна")]
    [SerializeField] private Boost _boostPrefab;
    [SerializeField] private Boost _falseBoostPrefab;

    [SerializeField] private PairedValue<float> _xZone;
    [SerializeField] private PairedValue<float> _yZone;
    [SerializeField] private PairedValue<float> _yDelta;
    [SerializeField] private PairedValue<float> _boostDistance;
    [SerializeField] private AnimationCurve[] _curves;
    
    [SerializeField] private float _minimumFlightTime;

    
    [Header("До и после зоны кол-во бустов, From > To")]
    [SerializeField] private PairedValue<int> _countTrueWays;
    [SerializeField] private int _countFalseWays;
    [SerializeField] private Transform _zoneSpawn;
    
    [SerializeField] private float _startFlightZ = 0f;
    

    private PlayerMovement _playerMovement;
    private float _minDistance;
    
    private List<List<Boost>> _trueWays = new();
    private List<List<Boost>> _falseWays = new();
    

    [Inject]
    public void Init(PlayerMovement playerMovement) {
        _playerMovement = playerMovement;
    }


    public List<Boost> GetRandomWay(float trueChance) {
        return Random.value <= trueChance ? _trueWays[Random.Range(0, _trueWays.Count)] 
            : _falseWays[Random.Range(0, _falseWays.Count)];
    }
    
    
    public void SpawnBoosts(Vector3 curiserPosition) {
        ClearAllBoosts();
        
        /* Акт в 5 этюдах:
        1. Самое легкое фейк бусты - каждый раз мы спавним просто цепочки до (minDist, cruiser-someValue) 
        2. Спавн в зоне выхода n2 правильных входов в бусты + некст до крейсера
        3. Спавн в начальной зоне n1 бустов, где n1>n2
        4. Соединение выхода из первой зоны с выходом во вторую зону
        */
        _minDistance = CalculateFalseTargetDistance();
        MoveVisualZone();
        for (int i = 0; i < _countFalseWays; i++) {
            // Тут тоже подумать до куда он может лететь, 
            Vector3 newFalsePosition = CalculateMinEndPosition();
            newFalsePosition.z = Random.Range(newFalsePosition.z, curiserPosition.z+100f);
            _falseWays.Add(SpawnBoostWays(_startFlightZ, newFalsePosition, _falseBoostPrefab, false));
        }

        // 2. После зоны
        if (curiserPosition.z - _minDistance < _boostDistance.To + _boostDistance.From) {
            Debug.Log("Задано большое значение _minimumFlightTime: " + _minimumFlightTime);
            return;
        }
        
        
        // float _newStartFlightZ = CalculateAfterZoneFirstPosition();
        // Debug.Log(_minDistance);
        // Debug.Log(_newStartFlightZ);
        for (int i = 0; i < _countTrueWays.To; i++) {
            _trueWays.Add(SpawnBoostWays(_minDistance, curiserPosition, _boostPrefab, false));
        }

        // foreach (var way in _trueWays) {
        //     Debug.Log("Кол-во бустов в пути: " +  way.Count);
        // }
        //
        // Debug.Log("Количество путей: " + _trueWays.Count);

        // До зоны
        for (int i = 0; i < _countTrueWays.From; i++) {
            Vector3 endPosition = _trueWays[Random.Range(0, _countTrueWays.To-1)][0].transform.position;
            _trueWays.Add(SpawnBoostWays(_startFlightZ, endPosition, _boostPrefab, true));
        }
        
        // _trueWays.Add(SpawnBoostWays(true, targetPosition));
        
        SpawnEntranceBoost();
    }

    private void MoveVisualZone() {
        Vector3 zonePos =  _zoneSpawn.position;
        zonePos.z = _minDistance;
        _zoneSpawn.position = zonePos;
    }


    private void ClearAllBoosts() {
        foreach (var chain in _trueWays) {
            foreach (var item in chain) {
                Destroy(item.gameObject);
            }
        }
        foreach (var chain in _falseWays) {
            foreach (var item in chain) {
                Destroy(item.gameObject);
            }
        }
        _trueWays.Clear();
        _falseWays.Clear();
    }

    
    
    private List<Boost> SpawnBoostWays(float initZ, Vector3 targetPosition, Boost boostPrefab, bool beforeZone) {
        if (targetPosition.z < initZ) {
            Debug.Log("Ебать ты крутой");
            throw new Exception();
        }
        
            
        
        List<float> spawnPoints = new List<float>();
        float currentPosition = initZ;
        Vector3 endPosition = targetPosition;


        bool firstBoost = true;
        
        while (currentPosition < endPosition.z) {
            float newSpawnPoint = Random.Range(_boostDistance.From, _boostDistance.To);
            // Первый буст дольше обычного чтоб игрок сдюжил
            if (firstBoost && beforeZone) {
                firstBoost = false;
                newSpawnPoint = _boostDistance.To;
            }
            currentPosition += newSpawnPoint;
            // Прям если в нужной точке буст то хуйня
            if (endPosition.z - currentPosition > _boostDistance.From) {
                spawnPoints.Add(currentPosition);
            }
        }
        
    
        // Сортируем для гарантии порядка
        spawnPoints.Sort();
        List<Boost> boost = new ();
        if (spawnPoints.Count == 0) {
            Debug.LogError("У тя 0 spawnPoints");
            throw new Exception();
        }
        
        // Точки спауна
        // firstBoost = true;
        float currentY = Random.Range(_yZone.To/2, _yZone.To);
        foreach (float zPos in spawnPoints) {
            float deltaY = Random.Range(_yDelta.From, _yDelta.To);
            currentY += deltaY;
            currentY = Mathf.Clamp(currentY, _yZone.From, _yZone.To);
            
            Vector3 spawnPosition = new Vector3(
                Random.Range(_xZone.From,_xZone.To), 
                currentY,                  
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

    
    
    private void SpawnEntranceBoost() {
        _playerMovement.SetBooster(_curves[0], _trueWays[^1][0].transform.position); // действует на игрока первым
    }


    private float CalculateFalseTargetDistance() {
        float speed = _playerMovement.PlayerSpeed;
        float z = speed * _minimumFlightTime;
        Debug.Log($"Минимальная точка падения {z}м. ");
        return z;
    }
    
    private Vector3 CalculateMinEndPosition() {
        float x = Random.Range(_xZone.From, _xZone.To);
        return new Vector3(x, 0f, _minDistance);
    }

    
    // После зоны входной буст
    private float CalculateAfterZoneFirstPosition() =>
        _minDistance + Random.Range(_boostDistance.From, _boostDistance.To);


    

}
