using System;
using Architecture_M;
using MirraSDK_M;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class PetStationViewAds : PetStationViewBase {

    [Header("Тип станции")]
    [SerializeField] private int _countToShowReward;
    [SerializeField] private TMP_Text _countTextVisual;
    [SerializeField] private Image _rewardProgress;
    
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetizationMirra;
    

    private void Awake() {
        _countTextVisual.text = $"{_showedReward} / {_countToShowReward}";
        _rewardProgress.fillAmount = (float)_showedReward / _countToShowReward;
    }



    protected override void TryAddPet() {
        _advertisingMonetizationMirra.InvokeRewarded(
            null,
            (isSuccess) => AddPetByReward(isSuccess)
        );
        
            
        // 1. Обычный метод
        bool MyMethod(bool value) {
            Debug.Log(value);
            return true;
        }

        // 2. Создаем делегат, указывающий на этот метод
        Func<bool, bool> myDelegate1 = MyMethod;


    }

    private void AddPetByReward(bool success) {
        if(!success) return;
        _showedReward++;
        Debug.Log($"Reward pet count = {_showedReward} / {_countToShowReward}");
        if (_showedReward == _countToShowReward) {
            // наградить 
            PetChance pet = GetRandomPet(_config);
            _petsManager.AddPet(pet.PetItemConfig);
            _petOpenView.ShowOpenPetView(pet);
            _showedReward = 0;
        }
        _countTextVisual.text = $"{_showedReward} / {_countToShowReward}";
        _rewardProgress.fillAmount = (float)_showedReward / _countToShowReward;
    }


   
}