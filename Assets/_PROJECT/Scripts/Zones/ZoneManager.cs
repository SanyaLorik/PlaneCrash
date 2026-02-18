using System;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ZoneManager : MonoBehaviour {
    [SerializeField] private Transform _cruiser;
    [SerializeField] private float _cruiserBaseSpawnDistance; // условно 500 или 1к за 1х
    [SerializeField] private PairedValue<float> _cruiserSpawnDistanceX;
    [SerializeField] private MoneyCube _moneyCube;
    
    
    
    public event Action<float> ChooseMultiplier;
    public event Action<float> ChooseBet;
    
    public float BetMultiplier { get; private set; }
    public float BetAmount { get; private set; }
    
    public float CruiserSpawnDistance { get; private set; } 
    public float DistanceToCruise { get; private set; } 

    private LevelBounds _levelBounds;
    private BoostSpawner _boostSpawner;

    
    [Inject] private PlayerStateManager _playerStateManager;
    
    [Inject]
    public void Init(BoostSpawner boostSpawner, LevelBounds levelBounds) {
        _boostSpawner = boostSpawner;
        _levelBounds = levelBounds;
    }
    


    private void Start() {
        _moneyCube.SetMoneyAmount(0);
        
    }


    public void ChangeBet(float newBet) {
        BetAmount = newBet;
        ChooseBet?.Invoke(BetAmount);
        _moneyCube.SetMoneyAmount(BetAmount,false);
    }


    public void ChangeMultiplier(float newMultiplier) {
        BetMultiplier = newMultiplier;

        if (newMultiplier == 0) {
            return;
        }

        CruiserSpawnDistance = BetMultiplier * _cruiserBaseSpawnDistance + _playerStateManager.StartFlightPositionZ;
        DistanceToCruise = BetMultiplier * _cruiserBaseSpawnDistance;
        
        Vector3 newCruiserSpawnPos = new Vector3(
            Random.Range(_cruiserSpawnDistanceX.From, _cruiserSpawnDistanceX.To), 
            0f, 
            CruiserSpawnDistance);
        
        _cruiser.position = newCruiserSpawnPos;

        
        newCruiserSpawnPos = _levelBounds.RecalculateCruiserY();
        _moneyCube.SetMoneyAmount(BetAmount * BetMultiplier);
        _boostSpawner.SpawnBoosts(newCruiserSpawnPos);
        ChooseMultiplier?.Invoke(BetMultiplier);
    }


}
