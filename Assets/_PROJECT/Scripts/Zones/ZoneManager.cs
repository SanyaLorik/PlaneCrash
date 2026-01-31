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
    
    
    
    public event Action<float> ChooseMultiplyer;
    public event Action<float> ChooseBet;
    
    public float CurrentMultiplyer { get; private set; }
    public float CurrentBet { get; private set; }
    
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
        CurrentBet = newBet;
        ChooseBet?.Invoke(CurrentBet);
        _moneyCube.SetMoneyAmount(CurrentBet,false);
    }


    public void ChangeMultiplyer(float newMultiplyer) {
        if (newMultiplyer < 0) {
            Debug.Log("Множитель не может быть < 0");
            return;
        }
        CurrentMultiplyer = newMultiplyer;

        if (newMultiplyer == 0) {
            return;
        }

        CruiserDistance = CurrentMultiplyer * _cruiserBaseSpawnDistance;
        Vector3 newCruiserSpawnPos = new Vector3(
            Random.Range(_cruiserSpawnDistanceX.From, _cruiserSpawnDistanceX.To), 
            0f, 
            CruiserDistance);
        
        _cruiser.position = newCruiserSpawnPos;
        Debug.Log($"Крейсер на {CruiserDistance}м");
        
        newCruiserSpawnPos = _levelBounds.RecalculateCruiserY();
        _moneyCube.SetMoneyAmount(CurrentBet*CurrentMultiplyer);
        _boostSpawner.SpawnBoosts(newCruiserSpawnPos);
        ChooseMultiplyer?.Invoke(CurrentMultiplyer);
    }


}
