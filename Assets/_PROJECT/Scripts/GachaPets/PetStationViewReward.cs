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

    private CancellationTokenSource _tokenSource;

    private void Awake() {
        StartNewWaitCycle();
    }

    private bool _allowToGetPet;


    
    private async UniTask WaitForRewardAsync(CancellationToken token) {
        int elapsedTimeSec = 0;
        SetFillAmount(0);
        while (!token.IsCancellationRequested && elapsedTimeSec < _timeToWaitSec) {
            await UniTask.WaitForSeconds(1, cancellationToken:token);
            elapsedTimeSec += 1;
            ///_timeToWaitText.text = (_timeToWaitSec - elapsedTimeSec) + "сек";
            _timeToWaitText.text = _localization.GetPrettyTime(_timeToWaitSec - elapsedTimeSec);
            SetFillAmount((float)elapsedTimeSec / _timeToWaitSec);
        }
        _timeToWaitText.text = _localization.TakeAPet;

        SetFillAmount(1);
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
    
    
    private void SetFillAmount(float percent) {
        _clockRectTransform.offsetMax = new Vector2(GetXPoseByPercent(percent), 0);
        Debug.Log(GetXPoseByPercent(percent));
        
    }

    private float GetXPoseByPercent(float percent) {
        float _xEnd = _parentRectTransform.rect.width;
        if (_xEnd < 0) {
            Debug.LogError("_xEnd < 0, Force UPDATE" );
            Canvas.ForceUpdateCanvases();
            _xEnd = _parentRectTransform.rect.width;
            Debug.LogError("_xEnd = " + _xEnd);
        }
        return -_xEnd * (1f - percent);
    }

   
}