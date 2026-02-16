using TMPro;
using UnityEngine;

public class PetStationViewPurchase : PetStationViewBase {
    [SerializeField] private TMP_Text _price;


    private void Awake() {
        _price.text = _formatter.ValuteFormatter(_config.Price);
    }


    protected override void AddPet() {
        Debug.Log("Buy pet");
        PetChance pet = GetRandomPet(_config);
        _bank.GiveMeYourFuckingMoneyNigga(_config.Price);
        _petsManager.AddPet(pet.PetItemConfig);
        _petOpenView.ShowOpenPetView(pet);
    }
    
}