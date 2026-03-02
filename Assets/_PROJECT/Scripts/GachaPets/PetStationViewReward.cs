using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PetStationViewReward : PetStationViewBase {
    [SerializeField] private int _timeToWaitSec;
    [SerializeField] private TMP_Text _timeToWaitText;
    [SerializeField] private RectTransform _clockRectTransform;
    [SerializeField] private RectTransform _parentRectTransform;

    [Inject] private LocalizationDataPC _localization;
    [Inject] private RectTransformHelper _fillAmounthMover;

    private CancellationTokenSource _tokenSource;

    private void Awake() {
        StartNewWaitCycle();
    }
    

    
    private async UniTask WaitForRewardAsync(CancellationToken token) {
        int elapsedTimeSec = 0;
        _fillAmounthMover.SetFillAmount(_clockRectTransform, _parentRectTransform, 0);
        _customTrigger.SetUnvailable();
        AllowToGetPet = false;
        while (!token.IsCancellationRequested && elapsedTimeSec < _timeToWaitSec) {
            await UniTask.WaitForSeconds(1, cancellationToken:token);
            elapsedTimeSec += 1;
            _timeToWaitText.text = _localization.GetPrettyTime(_timeToWaitSec - elapsedTimeSec);
            float percent = (float)elapsedTimeSec / _timeToWaitSec;
            _fillAmounthMover.SetFillAmount(_clockRectTransform, _parentRectTransform, percent);
        }
        _customTrigger.SetAvailable();
        _timeToWaitText.text = _localization.TakeAPet;

        _fillAmounthMover.SetFillAmount(_clockRectTransform, _parentRectTransform, 1);
        AllowToGetPet = true;
    }
    
    protected override void AddPet() {
        if(!AllowToGetPet) return;
        AllowToGetPet = false;
        PetChance pet = GetRandomPet(_config);
        _petsManager.AddPet(pet.PetItemConfig);
        _petOpenView.ShowOpenPetView(pet);
        _bank.Buy(0);
        
        StartNewWaitCycle();
    }

    private void StartNewWaitCycle() {
        _tokenSource = new CancellationTokenSource();
        WaitForRewardAsync(_tokenSource.Token).Forget();
    }
   
}