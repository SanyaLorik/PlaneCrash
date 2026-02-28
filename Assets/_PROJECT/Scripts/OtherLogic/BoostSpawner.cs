using System;
using System.Collections.Generic;
using System.Linq;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BoostSpawner : MonoBehaviour {
    
    [Header("Граница спауна")]
    [field: SerializeField] public PairedValue<float> BoostDistance { get; private set;  }
    [SerializeField] private Boost _boostPrefab;
    [SerializeField] private Boost _falseBoostPrefab;
    
    [SerializeField] private PairedValue<float> _yDelta;
    
    [SerializeField] private AnimationCurve[] _curves;
    
    [SerializeField] private float _xOffset;

    [field: SerializeField] public float YMinUpBoostCorrect { get; private set;  } = 5f;

    [Header("До и после зоны кол-во бустов, From > To")]
    [SerializeField] private PairedValue<int> _countTrueWays;
    [SerializeField] private int _countFalseWays;
    
    [SerializeField] private bool _setPlayerFalseWay;
    

    public float MinDistance { get; private set; }

    private List<List<Boost>> _trueWaysAfterZone = new();
    private List<List<Boost>> _trueWaysBeforeZone = new();
    private List<List<Boost>> _falseWays = new();
    
    private bool MinimumFlightTimeIsBig;

    public float YMinBoost { get; private set; }

    [Inject] private LevelBounds _levelBounds;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private PlayerStateManager _playerStateManager;
    
    [Inject] UpgradesCalculator _upgradesCalculator;
    [Inject] ObjectPoolManager _objectPoolManager;
    [Inject] private IPlayerStatsReadOnly _playerStats;
    [Inject] private DiContainer _container;
    
    
    private void OnEnable() {
        _playerStats.ChangeStats += RecalculateSafeDistance;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }    
    
    private void OnDisable() {
        _playerStats.ChangeStats -= RecalculateSafeDistance;
        _playerStateManager.ChangeState -= PlayerStateManagerOnChangeState;
    }

    private void Start() {
        YMinBoost = _levelBounds.MinY;
    }

    private float CalculateFalseTargetLength() {
        // Debug.Log($"CalculateFalseTargetDistance: MinimumFlightTime = {_upgradesCalculator.GetLuckyByLevel()}");
        float speed = _playerMovement.PlayerSpeed;
        float z = speed * _upgradesCalculator.GetLuckyByLevel();
        // Debug.Log($"Минимальная точка падения {z}м. ");
        if (z < BoostDistance.From) {
            Debug.LogWarning("Задано ооочень маленькое значение для зоны 45 секунд");
        }
        return z;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Cruisered || state == PlayerState.Grounded) {
            ClearAllBoosts();
        }
    }

    private void RecalculateSafeDistance() {
        MinDistance = CalculateFalseTargetLength() + _playerStateManager.StartFlightPositionZ;
        Debug.LogWarning($"MinDistance = {CalculateFalseTargetLength()} + {_playerStateManager.StartFlightPositionZ} метров");
    }


    public Boost GetRandomFirstBoost(float trueChance) {
        if (Random.value > trueChance && _falseWays[0].Count > 0) {
            return GetRandomFirstBoost(false);
        }
        return GetRandomFirstBoost(true);
    }   
    
    public List<Boost> GetAllBoosts() {
        return _trueWaysBeforeZone
            .Concat(_trueWaysAfterZone)
            .Concat(_falseWays)
            .SelectMany(list=>list)
            .ToList();
    }


    private Vector3 _cruiserPosition;
    public void SpawnBoosts(Vector3 cruiserPosition) {
        _cruiserPosition = cruiserPosition;
        MinimumFlightTimeIsBig = false;
        ClearAllBoosts();
        RecalculateSafeDistance();
        /* Акт в 5 этюдах:
        1. Самое легкое фейк бусты - каждый раз мы спавним просто цепочки до (minDist, cruiser-someValue) 
        2. Спавн в зоне выхода n2 правильных входов в бусты + некст до крейсера
        3. Спавн в начальной зоне n1 бустов, где n1>n2
        4. Соединение выхода из первой зоны с выходом во вторую зону
        */
        
        for (int i = 0; i < _countFalseWays; i++) {
            
            // Тут тоже подумать до куда он может лететь, 
            Vector3 newFalsePosition = CalculateMinEndPosition();
            newFalsePosition.z = Random.Range(newFalsePosition.z, cruiserPosition.z+100f);
            _falseWays.Add(SpawnBoostWays(_playerStateManager.StartFlightPositionZ, newFalsePosition, _falseBoostPrefab, false));
        }

        
        
        // 2. После зоны
        if (cruiserPosition.z - MinDistance < BoostDistance.To + BoostDistance.From) {
            Debug.Log("Задано большое значение MinimumFlightTime, все пути ведут к крейсеру " + _upgradesCalculator.GetLuckyByLevel());
            MinimumFlightTimeIsBig = true;
        }
        else {
            Debug.Log("MinimumFlightTime: " + _upgradesCalculator.GetLuckyByLevel());
            for (int i = 0; i < _countTrueWays.To; i++) {
                _trueWaysAfterZone.Add(SpawnBoostWays(MinDistance, cruiserPosition, _boostPrefab, false));
            }
        }
        
        for (int i = 0; i < _countTrueWays.From; i++) {
            Vector3 endPosition = !MinimumFlightTimeIsBig ? 
                _trueWaysAfterZone[Random.Range(0, _trueWaysAfterZone.Count)][0].transform.position 
                : 
                cruiserPosition;
            _trueWaysBeforeZone.Add(SpawnBoostWays(_playerStateManager.StartFlightPositionZ, endPosition, _boostPrefab, true));
        }
    }


    

    public void SpawnEntranceBoost() {
        if (_setPlayerFalseWay) {
            _playerMovement.SetBooster(_curves[0], _falseWays[0][0].transform.position); 
            return;
        }
        _playerMovement.SetBooster(_curves[0], GetRandomFirstBoost(true).transform.position);
    }

    private Boost GetRandomFirstBoost(bool trueBoost) {
        if (trueBoost) {
            return _trueWaysBeforeZone[0].Count != 0
            ? _trueWaysBeforeZone[0][0]
            : _trueWaysAfterZone[0][0];
        }

        if (_falseWays[0].Count != 0) {
            return _falseWays[0][0];
        }
        Debug.Log("Вернули всеровно правильную цепочку, неправильных бустов нет");
        return GetRandomFirstBoost(true);
    }


    private List<Boost> _rightBoosts = new();


    public List<Boost> GetRightBoosts() {
        if (_rightBoosts.Count == 0) {
            _rightBoosts = 
                _trueWaysBeforeZone
                    .Concat(_trueWaysAfterZone)
                    .SelectMany(list => list)
                    .OrderBy(boost => boost.transform.position.z)
                    .ToList();
        }
        return _rightBoosts;
    }

    public void SetPlayerToNextRightBooster(Vector3 playerPosition) {
        GetRightBoosts();

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

        Debug.LogWarning("Буста не нашли(, летим к крейсеру");
        _playerMovement.SetBooster(_rightBoosts[^2].randomTrajectory, _cruiserPosition);
    }


    private void ClearAllBoosts() {
        List<List<Boost>> boosts = new ();
        boosts.AddRange(_trueWaysAfterZone);
        boosts.AddRange(_trueWaysBeforeZone);
        boosts.AddRange(_falseWays);
        foreach (var chain in boosts) {
            foreach (var boost in chain) {
                if (boost.InPool) {
                    // Destroy(boost.gameObject);
                    boost.SetBoostDefault();
                    _objectPoolManager.ReturnObjectToPool(boost.gameObject, PoolType.Boost);
                }
                
            }
        }

        _trueWaysAfterZone.Clear();
        _trueWaysBeforeZone.Clear();
        _falseWays.Clear();
        _rightBoosts.Clear();
    }

    
    
    private List<Boost> SpawnBoostWays(float initZPos, Vector3 targetPosition, Boost boostPrefab, bool isBeforeZone) {
        if (targetPosition.z < initZPos) {
            Debug.Log("targetPosition.z < initZPos");
            throw new Exception();
        }
        
        List<float> spawnPoints = BoostsZPoints(initZPos, targetPosition, isBeforeZone);

 
        
        List<Boost> boost = new ();
        
        // Начальная высота буста
        float currentY = Random.Range(_levelBounds.MaxY/1.5f, _levelBounds.MaxY);
        foreach (float zPos in spawnPoints) {
            float deltaY = Random.Range(_yDelta.From, _yDelta.To);
            currentY += deltaY;
            currentY = Mathf.Clamp(currentY, _levelBounds.MinY + YMinUpBoostCorrect, _levelBounds.MaxY);
            
            Vector3 spawnPosition = new Vector3(
                Random.Range(_levelBounds.LeftX + _xOffset,_levelBounds.RightX - _xOffset), 
                currentY,                  
                zPos                  
            );
        
            // Boost newBoost = Instantiate(boostPrefab, spawnPosition, Quaternion.identity, transform);
            
            Boost newBoost = _objectPoolManager.Spawn<Boost>(boostPrefab.gameObject, spawnPosition, PoolType.Boost);
            newBoost.InPool = true;
            
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
            float newSpawnPoint = Random.Range(BoostDistance.From, BoostDistance.To);
            // Первый буст дольше обычного чтоб игрок сдюжил
            if (firstBoost && isBeforeZone) {
                firstBoost = false;
                // Рандомное значение просто для отдаления
                newSpawnPoint = BoostDistance.From * Random.Range(1.2f, 1.7f); 
            }
            initZPos += newSpawnPoint;
            // Прям если в нужной точке буст то хуйня
            if (targetPosition.z - initZPos > BoostDistance.From) {
                spawnPoints.Add(initZPos);
            }
        }
    
        // Сортируем для гарантии порядка
        spawnPoints.Sort();
        if (spawnPoints.Count == 0) {
            Debug.LogWarning(isBeforeZone
                ? "0 spawnPoints до зоны"
                : "0 spawnPoints после зоны мб там большое значение и все пути к крейсеру");
        }

        return spawnPoints;
    }


  
    
    private Vector3 CalculateMinEndPosition() {
        float x = Random.Range(_levelBounds.LeftX, _levelBounds.RightX);
        return new Vector3(x, YMinBoost, MinDistance);
    }
    

}
