using MirraSDK_M;
using TMPro;
using UnityEngine;
using Zenject;

public class SkinItemViewAdv : SkinItemViewBase {
    [SerializeField] private int _countsToShowAdv;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private RectTransform _advProgress;
    [SerializeField] private RectTransform _advProgressParent;
    private int _countShowAdv = 0;
    
    
    [Inject] private AdvertisingMonetizationMirra _advertisingMonetizationMirra;
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        if (SkinIsBought()) {
            if (!SkinIsWeared()) {
                _delayedTrigger.DelayedTriggerAction(WearSkin);
            }
        }
        else {
            _delayedTrigger.DelayedTriggerAction(ShowAdv);
        }
    }

    private void ShowAdv() {
        _advertisingMonetizationMirra.InvokeRewarded(
            null,
            (isSuccess) => 
            {
                if (isSuccess) {
                    UpdateCountsShowAdv();
                }
            }
        );
    }
    
    private void UpdateCountsShowAdv() {
        _countShowAdv++;
        if (_countShowAdv == _countsToShowAdv) {
            GetNewSkin();
            WearSkin();
            HideGetVisual();
        }
        else {
            SetProgress();
        }
    }
    

    private void SetProgress() {
        _progressText.text = $"{_countShowAdv}/{_countsToShowAdv}";
        float progress = (float)_countShowAdv / _countsToShowAdv;
        _fillAmounthMover.SetFillAmount(_advProgress, _advProgressParent, progress);
    }

    protected override void InitSpecific() {
        SetProgress();
    }

    protected override void OnEnableSpecific() { }
}