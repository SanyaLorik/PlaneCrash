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
        if (_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            _delayedTrigger.DelayedTriggerAction(WearNewSkin);
        }
        else {
            _delayedTrigger.DelayedTriggerAction(ShowAdv);
        }
    }

    protected override void GetNewSkin() {
        _saver.GetSave.AddNewSkin(SkinItemConfig.Id);
        WearNewSkin();
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
    
    
}