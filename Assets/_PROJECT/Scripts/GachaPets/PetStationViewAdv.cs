using MirraSDK_M;
using TMPro;
using UnityEngine;
using Zenject;


public class PetStationViewAdv : PetStationViewBase {

    [Header("Тип станции")]
    [SerializeField] private int _countToShowReward;
    [SerializeField] private TMP_Text _countTextVisual;
    [SerializeField] private RectTransform _rewardProgress;
    [SerializeField] private RectTransform _rewardProgressParent;
    
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetizationMirra;
    

    
    protected override void StartInit() {
        _countTextVisual.text = $"{_showedReward} / {_countToShowReward}";
        RectTransformHelper.SetFillAmount(_rewardProgress, _rewardProgressParent,  0);
    }

    protected override void AddPet() {
        _advertisingMonetizationMirra.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    AddPetByReward();
                }
            }
        );
    }

    private void AddPetByReward() {
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
        float percent = (float)_showedReward / _countToShowReward;
        RectTransformHelper.SetFillAmount(_rewardProgress, _rewardProgressParent,  percent);
        _bank.Buy(0);
    }
}