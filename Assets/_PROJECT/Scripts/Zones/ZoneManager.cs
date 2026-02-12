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
    
    public float CruiserDistance { get; private set; }


    private LevelBounds _levelBounds;
    private BoostSpawner _boostSpawner;

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
        if (newMultiplier < 0) {
            Debug.Log("Множитель не может быть < 0");
            return;
        }
        BetMultiplier = newMultiplier;

        if (newMultiplier == 0) {
            return;
        }

        CruiserDistance = BetMultiplier * _cruiserBaseSpawnDistance;
        Vector3 newCruiserSpawnPos = new Vector3(
            Random.Range(_cruiserSpawnDistanceX.From, _cruiserSpawnDistanceX.To), 
            0f, 
            CruiserDistance);
        
        _cruiser.position = newCruiserSpawnPos;
        Debug.Log($"Крейсер на {CruiserDistance}м");
        
        newCruiserSpawnPos = _levelBounds.RecalculateCruiserY();
        _moneyCube.SetMoneyAmount(BetAmount * BetMultiplier);
        _boostSpawner.SpawnBoosts(newCruiserSpawnPos);
        ChooseMultiplier?.Invoke(BetMultiplier);
    }


}
