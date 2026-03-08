using TMPro;
using UnityEngine;

public class SkinItemViewPrice : SkinItemViewBase {
    [SerializeField] private TextMeshProUGUI _priceText;
    
    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;

        if (SkinIsBought()) {
            if (!SkinIsWeared()) {
                _delayedTrigger.DelayedTriggerAction(WearSkin);
            }
        }
        else if (_playerBank.CanBuy(SkinItemConfig.Price)) {
            _delayedTrigger.DelayedTriggerAction(BuyNewSkin);
        }
    }

    private void BuyNewSkin() {
        _playerBank.Buy(SkinItemConfig.Price);
        GetNewSkin();
        WearSkin();
    }    
    
    protected override void InitSpecific() {
        _priceText.text = _formatter.ValuteFormatter(SkinItemConfig.Price);
    }
    
}