using TMPro;
using UnityEngine;

public class SkinItemViewPrice : SkinItemViewBase {
    [SerializeField] private TextMeshProUGUI _priceText;
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;

        if (SkinIsBought()) {
            if (!SkinIsWeared()) {
                _delayedTrigger.DelayedTriggerAction(WearNewSkin);
            }
        }
        else if (_playerBank.CanBuy(SkinItemConfig.Price)) {
            _delayedTrigger.DelayedTriggerAction(GetNewSkin);
        }
    }



    protected override void GetNewSkin() {
        _saver.GetSave.AddNewSkin(SkinItemConfig.Id);
        WearNewSkin();
        _playerBank.Buy(SkinItemConfig.Price);
        HideGetVisual();
    }
    
    protected override void InitSpecific() {
        _playerBank.BankChanged += CheckBuy;
        CheckBuy(_playerBank.PlayerCapital);
        _priceText.text = _formatter.ValuteFormatter(SkinItemConfig.Price);
    }
    
    private void CheckBuy(long capital) {
        if (capital < SkinItemConfig.Price && !_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            _delayedTrigger.SetUnvailable();
        }
        else {
            _delayedTrigger.SetAvailable();
        }
    }
    
}