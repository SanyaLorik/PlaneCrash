using System.Collections;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class MoneySpawnManager : MonoBehaviour {
    [Range(0f,1f), SerializeField] private float _chanceToSpawn;
    [SerializeField] private MoneyObject _moneyPrefab;
    [SerializeField] private PairedValue<float> _xDistance;
    [SerializeField] private PairedValue<int> _moneyAmountDiapasone;
    
    [Header("Range 0 to 1!!!")]
    [SerializeField] private PairedValue<float> _boostProgress;


    private PlayerStateManager _playerStateManager;
    [Inject] private PlayerConfig _playerConfig;
    [Inject] private BoostSpawner _boostSpawner;
    [Inject]private LevelBounds _levelBounds;
    
    [Inject] private DiContainer _container;
    
    [Inject]
    public void Init(PlayerStateManager playerStateManager) {
        _playerStateManager = playerStateManager;
        _playerStateManager.ChangeState += PlayerStateManagerOnChangeState;
    }

    private void PlayerStateManagerOnChangeState(PlayerState state) {
        if (state == PlayerState.Flight) {
            StartCoroutine(SpawnMoney(_boostSpawner.GetAllBoosts()));
        }
    }


    private List<MoneyObject> _spawnedMoneys = new();
    private IEnumerator SpawnMoney(List<Boost> _boosts) {
        ClearMoneyObjects();
        // Debug.Log("Spawn Money");
        foreach (var boost in _boosts) {
            if (Random.value > _chanceToSpawn) continue;
            
            Vector3 spawnPos = GetPositionOnBoost(boost);

            MoneyObject newMoney = Instantiate(_moneyPrefab, spawnPos, Quaternion.identity, transform);
            _container.Inject(newMoney);
            newMoney.SetMoneyAmount(Random.Range(_moneyAmountDiapasone.From, _moneyAmountDiapasone.To));
            _spawnedMoneys.Add(newMoney); 
            yield return null;
        }
    }
    
    private Vector3 GetPositionOnBoost(Boost boost) {
        float t = Random.Range(_boostProgress.From, _boostProgress.To);
        t = Mathf.Clamp01(t);

        float height = boost.randomTrajectory.Evaluate(t) * _playerConfig.JumpHeight;

        float x = boost.nextBooster.x + Random.Range(_xDistance.From, _xDistance.To);
        float y = Mathf.Lerp(boost.transform.position.y, boost.nextBooster.y, t) + height;
        float z = Mathf.Lerp(boost.transform.position.z, boost.nextBooster.z, t);

        Vector3 spawnPos = new Vector3(x, y, z);
        spawnPos = _levelBounds.ClampPosition(spawnPos);
        // Debug.Log($"Money spawn position: {spawnPos}");
        
        return spawnPos;
    }


    private void ClearMoneyObjects() {
        foreach (var money in _spawnedMoneys) {
            Destroy(money.gameObject);
        }
        _spawnedMoneys.Clear();
    }
    
    
    
    
}
