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
    
    [SerializeField] private float _minimumFlightTimeDefault;

    private float MinimumFlightTime => _minimumFlightTimeDefault * _playerStats.LuckyMultiplier;
    
    [Header("До и после зоны кол-во бустов, From > To")]
    [SerializeField] private PairedValue<int> _countTrueWays;
    [SerializeField] private int _countFalseWays;
    [SerializeField] private Transform _zoneSpawn;
    
    [SerializeField] private float _startFlightZ = 0f;
    [SerializeField] private bool _setPlayerFalseWay;
    

    private PlayerMovement _playerMovement;
    private IPlayerStatsReadOnly _playerStats;
    private float _minDistance;
    
    private List<List<Boost>> _trueWaysAfterZone = new();
    private List<List<Boost>> _trueWaysBeforeZone = new();
    private List<List<Boost>> _falseWays = new();
    

    [Inject]
    public void Init(PlayerMovement playerMovement, IPlayerStatsReadOnly playerStats) {
        _playerMovement = playerMovement;
        _playerStats =  playerStats;
    }


    public List<Boost> GetRandomWay(float trueChance) {
        if (Random.value > trueChance) {
            return _falseWays[Random.Range(0, _falseWays.Count)];
        }
        List<Boost> trueList = _trueWaysBeforeZone[Random.Range(0, _trueWaysBeforeZone.Count)];
        trueList.AddRange(_trueWaysAfterZone[0]);
        return trueList;
    }



    private bool MinimumFlightTimeIsBig;
    public void SpawnBoosts(Vector3 curiserPosition) {
        Debug.Log("Генерация бустов");
        MinimumFlightTimeIsBig = false;
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
            newFalsePosition.z = Random.Range(newFalsePosition.z, newFalsePosition.z + 500f);
            _falseWays.Add(SpawnBoostWays(_startFlightZ, newFalsePosition, _falseBoostPrefab, false));
        }

        // 2. После зоны
        if (curiserPosition.z - _minDistance < _boostDistance.To + _boostDistance.From) {
            Debug.Log("Задано большое значение MinimumFlightTime, все пути ведут к крейсеру " + MinimumFlightTime);
            MinimumFlightTimeIsBig = true;
        }
        else {
            Debug.Log("MinimumFlightTime: " + MinimumFlightTime);
            for (int i = 0; i < _countTrueWays.To; i++) {
                _trueWaysAfterZone.Add(SpawnBoostWays(_minDistance, curiserPosition, _boostPrefab, false));
            }
        }
        
        for (int i = 0; i < _countTrueWays.From; i++) {
            Vector3 endPosition = !MinimumFlightTimeIsBig ? 
                _trueWaysAfterZone[Random.Range(0, _trueWaysAfterZone.Count)][0].transform.position 
                : 
                curiserPosition;
            _trueWaysBeforeZone.Add(SpawnBoostWays(_startFlightZ, endPosition, _boostPrefab, true));
        }
    }

    public void SpawnEntranceBoost() {
        if (_setPlayerFalseWay) {
            _playerMovement.SetBooster(_curves[0], _falseWays[0][0].transform.position); // действует на игрока первым
            return;
        }
        _playerMovement.SetBooster(_curves[0], _trueWaysBeforeZone[0][0].transform.position); // действует на игрока первым
    }
    
    
    
    private void MoveVisualZone() {
        Vector3 zonePos =  _zoneSpawn.position;
        zonePos.z = _minDistance;
        _zoneSpawn.position = zonePos;
    }


    private void ClearAllBoosts() {
        List<List<Boost>> boosts = new ();
        boosts.AddRange(_trueWaysAfterZone);
        boosts.AddRange(_trueWaysBeforeZone);
        boosts.AddRange(_falseWays);
        foreach (var chain in boosts) {
            foreach (var item in chain) {
                Destroy(item.gameObject);
            }
        }

        _trueWaysAfterZone.Clear();
        _trueWaysBeforeZone.Clear();
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


    private float CalculateFalseTargetDistance() {
        Debug.Log($"CalculateFalseTargetDistance: MinimumFlightTime = {MinimumFlightTime}");
        float speed = _playerMovement.PlayerSpeed;
        float z = speed * MinimumFlightTime;
        Debug.Log($"Минимальная точка падения {z}м. ");
        return z;
    }
    
    private Vector3 CalculateMinEndPosition() {
        float x = Random.Range(_xZone.From, _xZone.To);
        return new Vector3(x, 0f, _minDistance);
    }


}
