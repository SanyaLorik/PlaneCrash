using TMPro;
using UnityEngine;

public class PetStationViewPurchase : PetStationViewBase {
    [SerializeField] private TMP_Text _priceText;


    private void Awake() {
        _priceText.text = _formatter.ValuteFormatter(_config.Price);
    }

    private void OnEnable() {
        _bank.BankChanged += BankOnBankChanged;
    }

    private void BankOnBankChanged(long obj) {
        CheckAvailable();
    }
    

    protected override void StartInit() {
        CheckAvailable();
    }

    private void CheckAvailable() {
        if (_bank.PlayerCapital < _config.Price) {
            _customTrigger.SetUnvailable();
            AllowToGetPet = false;
        }
        else {
            _customTrigger.SetAvailable();
            AllowToGetPet = true;
        }
    }
    

    protected override void AddPet() {
        Debug.Log("Buy pet");
        PetChance pet = GetRandomPet(_config);
        _bank.Buy(_config.Price);
        _petsManager.AddPet(pet.PetItemConfig);
        _petOpenView.ShowOpenPetView(pet);
    }
    
}