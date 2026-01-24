using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    [SerializeField] private PairedValue<float> _yZone;
    [SerializeField] private PairedValue<float> _yDelta;
    [SerializeField] private PairedValue<float> _boostDistance;
    [SerializeField] private AnimationCurve[] _curves;
    
    [SerializeField] private float _xOffset;
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
    private LevelBounds _levelBounds;
    private float _minDistance;
    
    private List<List<Boost>> _trueWaysAfterZone = new();
    private List<List<Boost>> _trueWaysBeforeZone = new();
    private List<List<Boost>> _falseWays = new();
    
    private bool MinimumFlightTimeIsBig;
    
    
    
    private DiContainer _container;
    [Inject]
    public void Init(PlayerMovement playerMovement, IPlayerStatsReadOnly playerStats, LevelBounds levelBounds, DiContainer container) {
        _playerMovement = playerMovement;
        _playerStats =  playerStats;
        _levelBounds = levelBounds;
        _container = container;
    }


    public List<Boost> GetRandomWay(float trueChance) {
        if (Random.value > trueChance) {
            return _falseWays[Random.Range(0, _falseWays.Count)];
        }
        List<Boost> trueList = _trueWaysBeforeZone[Random.Range(0, _trueWaysBeforeZone.Count)];
        if (_trueWaysAfterZone.Count > 0) {
            trueList.AddRange(_trueWaysAfterZone[Random.Range(0, _trueWaysAfterZone.Count)]);
        }
        return trueList;
    }   
    
    public List<Boost> GetAllBoosts() {
        return _trueWaysBeforeZone
            .Concat(_trueWaysAfterZone)
            .Concat(_falseWays)
            .SelectMany(list=>list)
            .ToList();
    }



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
            newFalsePosition.z = Random.Range(newFalsePosition.z, curiserPosition.z+100f);
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


    private List<Boost> _rightBoosts = new ();
    public void SetPlayerToNextRightBooster(Vector3 playerPosition) {
        if (_rightBoosts.Count == 0) {
            _rightBoosts = 
                _trueWaysBeforeZone
                    .Concat(_trueWaysAfterZone)
                    .SelectMany(list => list)
                    .OrderBy(boost => boost.transform.position.z)
                    .ToList();
        }

        Debug.Log("Правильные бусты:");
        foreach (var rightBoost in _rightBoosts) {
            Debug.Log(rightBoost.transform.position.z);
        }
        

        for (int i = 0; i < _rightBoosts.Count; i++) {
            if (_rightBoosts[i].transform.position.z > playerPosition.z + 100f) {
                if (i > 0) {
                    Debug.Log("Нашли первый буст следующий после игрока" + _rightBoosts[i].transform.position);
                    _playerMovement.SetBooster(_rightBoosts[i-1].randomTrajectory, _rightBoosts[i-1].nextBooster);
                    return;
                }
            }
        }
        Debug.LogWarning("Буста не нашли(");
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
        _rightBoosts.Clear();
    }

    
    
    private List<Boost> SpawnBoostWays(float initZPos, Vector3 targetPosition, Boost boostPrefab, bool isBeforeZone) {
        if (targetPosition.z < initZPos) {
            Debug.Log("Ебать ты крутой: targetPosition.z < initZPos");
            throw new Exception();
        }
        
        List<float> spawnPoints = BoostsZPoints(initZPos, targetPosition, isBeforeZone);

 
        
        List<Boost> boost = new ();
        
        // Начальная высота буста
        float currentY = Random.Range(_yZone.To/1.5f, _yZone.To);
        foreach (float zPos in spawnPoints) {
            float deltaY = Random.Range(_yDelta.From, _yDelta.To);
            currentY += deltaY;
            currentY = Mathf.Clamp(currentY, _yZone.From, _yZone.To);
            
            Vector3 spawnPosition = new Vector3(
                Random.Range(_levelBounds.LeftX + _xOffset,_levelBounds.RightX - _xOffset), 
                currentY,                  
                zPos                  
            );
        
            Boost newBoost = Instantiate(boostPrefab, spawnPosition, Quaternion.identity);
            _container.Inject(newBoost);
            boost.Add(newBoost); 
        }
        for (int i = 0; i < boost.Count; i++) {
            if (i != boost.Count - 1) {
                boost[i].nextBooster = boost[i + 1].transform.position;
                boost[i].randomTrajectory = _curves[Random.Range(0, _curves.Length)];
                // Debug.Log("Следующий буст в " + boost[i].nextBooster.z);
            }
            else {
                boost[i].nextBooster = targetPosition;
                boost[i].randomTrajectory = _curves[1];
                // Debug.Log("Конечный буст в  " + targetPosition.z);
            }
        }
        return boost;
    }

    private List<float> BoostsZPoints(float initZPos, Vector3 targetPosition, bool isBeforeZone) {
        List<float> spawnPoints = new List<float>();
        bool firstBoost = true;
        
        while (initZPos < targetPosition.z) {
            float newSpawnPoint = Random.Range(_boostDistance.From, _boostDistance.To);
            // Первый буст дольше обычного чтоб игрок сдюжил
            if (firstBoost && isBeforeZone) {
                firstBoost = false;
                newSpawnPoint = _boostDistance.From * Random.Range(1.2f, 1.7f);
            }
            initZPos += newSpawnPoint;
            // Прям если в нужной точке буст то хуйня
            if (targetPosition.z - initZPos > _boostDistance.From) {
                spawnPoints.Add(initZPos);
            }
        }
    
        // Сортируем для гарантии порядка
        spawnPoints.Sort();
        if (spawnPoints.Count == 0) {
            Debug.LogError("У тя 0 spawnPoints");
            throw new Exception();
        }

        return spawnPoints;
    }


    private float CalculateFalseTargetDistance() {
        Debug.Log($"CalculateFalseTargetDistance: MinimumFlightTime = {MinimumFlightTime}");
        float speed = _playerMovement.PlayerSpeed;
        float z = speed * MinimumFlightTime;
        Debug.Log($"Минимальная точка падения {z}м. ");
        return z;
    }
    
    private Vector3 CalculateMinEndPosition() {
        float x = Random.Range(_levelBounds.LeftX, _levelBounds.RightX);
        return new Vector3(x, 0f, _minDistance);
    }


}
