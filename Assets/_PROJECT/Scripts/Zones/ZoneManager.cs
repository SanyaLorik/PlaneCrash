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
    
    
    
    public event Action<float> OnChooseMultiplyer;
    public event Action<float> OnChooseBet;
    
    public float CurrentMultiplyer { get; private set; }
    public float CurrentBet { get; private set; }
    
    public static ZoneManager Instance { get; private set; }
    public float CruiserDistance { get; private set; }
    
    
        
    private BoostSpawner _boostSpawner;

    [Inject]
    public void Init(BoostSpawner boostSpawner) {
        _boostSpawner = boostSpawner;
    }
    
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("2 Zone Manager");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        _moneyCube.SetMoneyAmount(0);
        Debug.LogWarning("_moneyCube.SetMoneyAmount(0)");
    }


    public void ChangeBet(float newBet) {
        CurrentBet = newBet;
        OnChooseBet?.Invoke(CurrentBet);
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
        
        _moneyCube.SetMoneyAmount(CurrentBet*CurrentMultiplyer);
        _boostSpawner.SpawnBoosts(newCruiserSpawnPos);
        OnChooseMultiplyer?.Invoke(CurrentMultiplyer);
    }
    
    
}
