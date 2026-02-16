using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PetStationViewAds : PetStationViewBase {

    [Header("Тип станции")]
    [SerializeField] private int _countToShowReward;
    [SerializeField] private TMP_Text _countTextVisual;
    [SerializeField] private Image _rewardProgress;
    
    

    private void Awake() {
        _countTextVisual.text = $"{_showedReward} / {_countToShowReward}";
        _rewardProgress.fillAmount = (float)_showedReward / _countToShowReward;
    }

    


    protected override void AddPet() {
        // Просмотрел рекламу допустим
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