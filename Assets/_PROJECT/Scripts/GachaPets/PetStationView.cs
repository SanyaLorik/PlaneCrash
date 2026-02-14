using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public class EntityView {
    public Image Icon;
    public TMP_Text Percentage;
}

[Serializable]
public enum PetStationType {
    Reward,
    Purchase
}


public class PetStationView : MonoBehaviour {

    [Header("Тип станции")]
    [SerializeField] private PetStationType _petStationType;
    [SerializeField] private DelayedTrigger _customTrigger;
    [SerializeField] private PetStationConfig _config;
    [SerializeField] private EntityView[] _views;
    [SerializeField] private TMP_Text _price;

    
    [Inject] private NumberFormatter _formatter;
    [Inject] private PlayerBank _bank;
    [Inject] private PetOpenView _petOpenView;
    [Inject] private PetsManager _petsManager;

    
    private float _divider;
    private void Start() {
        _divider = ChanceSum(_config.Pets);
        Initialize();
    }

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        if (_petStationType == PetStationType.Purchase) {
            if (_bank.CanBuy(_config.Price)) {
                _customTrigger.DelayedTriggerAction(BuyPet);
            }
            else {
                Debug.Log("Недостаточно средств");
            }
        }
        else {
            Debug.Log("Награды в разработке...");
        }
    }

    
    private void OnTriggerExit(Collider collider) {
        if (!collider.TryGetComponent(out PlayerMovement _)) return;
        _customTrigger.CancelTriggerAction();
        Debug.Log("Операция по получению пета отменена");
    }
    
    private void Initialize() {
        for (int i = 0; i < _config.Pets.Length; i++) {
            _views[i].Icon.sprite = _config.Pets[i].PetItemConfig.Sprite;
            _views[i].Percentage.text = $"{_config.Pets[i].Chance / _divider * 100f:0.#}  %";
        }

        _price.text = _formatter.ValuteFormatter(_config.Price);
    }

    private void BuyPet() {
        Debug.Log("Buy pet");
        PetChance pet = GetRandomPet(_config);
        _bank.GiveMeYourFuckingMoneyNigga(_config.Price);
        _petsManager.DoPurchase(pet.PetItemConfig);
        _petOpenView.ShowOpenPetView(pet);
    }

    private PetChance GetRandomPet(PetStationConfig config) {
        float random = Random.Range(0f, _divider);
        float cumulative = 0f;

        foreach (var pet in config.Pets) {
            cumulative += pet.Chance;
            if (random <= cumulative)
                return pet;
        }
        
        return config.Pets[^1];
    }

    

    private float ChanceSum(PetChance[] pets) {
        float sum = 0f;
        foreach (var petChance in pets) {
            sum+= petChance.Chance;
        }
        return sum;
    }
}