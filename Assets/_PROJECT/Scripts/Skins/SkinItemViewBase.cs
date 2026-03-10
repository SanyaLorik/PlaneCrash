using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public abstract class SkinItemViewBase : MonoBehaviour {
    [field: SerializeField] public SkinItemConfig SkinItemConfig { get; private set; }
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _wearText;
    [SerializeField] protected DelayedTrigger _delayedTrigger;
    
    [SerializeField] protected GameObject _advContainer;
    [SerializeField] protected GameObject _priceContainer;
    
    
    [Inject] protected NumberFormatter _formatter;
    [Inject] protected LocalizationDataPC _localization; 
    [Inject] protected RectTransformHelper _fillAmounthMover;
    [Inject] protected PlayerSkinInventory _playerSkinInventory;
    [Inject] protected PlayerBank _playerBank;

    protected abstract void InitSpecific();

    private void OnEnable() {
        _playerSkinInventory.SkinUnlocked += HidePurchaseInfo;
        _playerSkinInventory.SkinEquipped += SkinEquippedCheck;
        _playerBank.BankChanged += PlayerBankOnBankChanged;
    }

    private void SkinEquippedCheck(SkinItemConfig skin) {
        if (skin.Id != SkinItemConfig.Id) {
            _wearText.text = string.Empty;
        }
    }

    private void PlayerBankOnBankChanged(long amount) {
        if (amount >= SkinItemConfig.Price) {
            _delayedTrigger.SetAvailable();
        }
        else {
            _delayedTrigger.SetUnvailable();
        }
    }



    private void HidePurchaseInfo(SkinItemConfig skin) {
        if(skin.Id != SkinItemConfig.Id) return;
        if (_priceContainer.activeSelf) {
            _priceContainer.DisactiveSelf();
        }

        if (_advContainer.activeSelf) {
            _advContainer.DisactiveSelf();
        }
    }


    private void Start() {
        InitSkinData();
    }

    private void InitSkinData() {
        GetTextLocalizated();

        // Скин есть
        if (_playerSkinInventory.SkinIsBought(SkinItemConfig.Id)) {
            HideGetVisual();
            
            if(_playerSkinInventory.CurrentSkinId == SkinItemConfig.Id) {
                WearSkin();
            }
        }
        // Скина НЭД
        else {
            if (SkinItemConfig.IsAdv) {
                _advContainer.ActiveSelf();
                _priceContainer.DisactiveSelf();
            }
            else {
                _priceContainer.ActiveSelf();
                _advContainer.DisactiveSelf();
            }
        }
        InitSpecific();
    }

    private void GetTextLocalizated() {
        _wearText.text = _localization.IsWeared;
        _nameText.text = _localization.GetTranslatedName(SkinItemConfig.Id, _localization.SkinNameTranslates);
    }


    protected void WearSkin() {
        Debug.Log("Надевание скина " + _localization.GetTranslatedName(SkinItemConfig.Id, _localization.SkinNameTranslates));
        _playerSkinInventory.EquipSkin(SkinItemConfig);
        _wearText.ActiveSelf();
    }
    
    protected void GetNewSkin() {
        _playerSkinInventory.UnlockSkin(SkinItemConfig);
    }

    protected void HideGetVisual() {
        _priceContainer.DisactiveSelf();
        _advContainer.DisactiveSelf();
    }
    
    protected bool SkinIsBought() {
        return _playerSkinInventory.SkinIsBought(SkinItemConfig.Id);
    }
    
    
    protected bool SkinIsWeared() {
        return _playerSkinInventory.CurrentSkinId == SkinItemConfig.Id;
    }
    

    
    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _delayedTrigger.CancelTriggerAction();
    }

}