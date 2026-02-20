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
    [SerializeField] private Image _clock;

    [Inject] private LocalizationDataPC _localization;

    private CancellationTokenSource _tokenSource;

    private void Awake() {
        StartNewWaitCycle();
    }

    private bool _allowToGetPet;


    
    private async UniTask WaitForRewardAsync(CancellationToken token) {
        int elapsedTimeSec = 0;
        _clock.fillAmount = 0;
        while (!token.IsCancellationRequested && elapsedTimeSec < _timeToWaitSec) {
            await UniTask.WaitForSeconds(1, cancellationToken:token);
            elapsedTimeSec += 1;
            ///_timeToWaitText.text = (_timeToWaitSec - elapsedTimeSec) + "сек";
            _timeToWaitText.text = _localization.GetPrettyTime(_timeToWaitSec - elapsedTimeSec);
            _clock.fillAmount = (float)elapsedTimeSec / _timeToWaitSec;
        }
        _timeToWaitText.text = _localization.TakeAPet;

        _clock.fillAmount = 1;
        _allowToGetPet = true;
    }
    
    protected override void TryAddPet() {
        if(!_allowToGetPet) return;
        _allowToGetPet = false;
        PetChance pet = GetRandomPet(_config);
        _petsManager.AddPet(pet.PetItemConfig);
        _petOpenView.ShowOpenPetView(pet);
        
        StartNewWaitCycle();
    }

    private void StartNewWaitCycle() {
        _tokenSource = new CancellationTokenSource();
        WaitForRewardAsync(_tokenSource.Token).Forget();
    }
}