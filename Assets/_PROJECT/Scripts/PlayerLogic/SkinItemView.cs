using Architecture_M;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkinItemView : MonoBehaviour {
    [field: SerializeField] public SkinItemConfig SkinItemConfig { get; private set; }
    //  либо через зенджект
    // [field: SerializeField] public SkinIdToNameConfig _skinIdToNameConfig { get; private set; }

    // FOR NOW ID, LATER ID->NAME CONVERTER
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private DelayedTrigger _delayedTrigger;
    [SerializeField] private Image _moneyIcon;

    [Inject] private NumberFormatter _formatter;
    [Inject] private IGameSave<GameSavePC> _saver; 
    [Inject] private LocalizationDataPC _localization; 
    [Inject] private PlayerBank _playerBank;
    [Inject] private PlayerSkinWear _playerSkinWear;

    
    public void InitSkinData() {
        // _nameText.text = GetNameById();
        _nameText.text = _localization.GetSkinName(SkinItemConfig.Id);
        if (_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            _priceText.text = string.Empty;
            _moneyIcon.DisactiveSelf();
        }
        else {
            _priceText.text = _formatter.ValuteFormatter(SkinItemConfig.Price);
        }
        
        if(_saver.GetSave.SkinWearId == SkinItemConfig.Id) {
            WearNewSkin();
        }
        _playerBank.BankChanged += PlayerBankOnBankChanged;
        _playerSkinWear.NewSkinWear += CheckSelect;
        PlayerBankOnBankChanged(_playerBank.PlayerCapital);
    }

    
    
    private void CheckSelect() {
        if (_saver.GetSave.SkinIsBought(SkinItemConfig.Id) && _saver.GetSave.SkinWearId != SkinItemConfig.Id) {
            _priceText.text = string.Empty;
        }
    }

    private void PlayerBankOnBankChanged(long capital) {
        Debug.Log(_saver.GetSave.SkinIsBought(SkinItemConfig.Id));
        if (capital < SkinItemConfig.Price && !_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            _delayedTrigger.SetUnvailable();
        }
        else {
            _delayedTrigger.SetAvailable();
        }

        CheckSelect();
    }
  

    private void OnTriggerEnter(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        
        if (_playerBank.CanBuy(SkinItemConfig.Price) && !_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            _delayedTrigger.DelayedTriggerAction(BuyNewSkin);
        }

        else if (_saver.GetSave.SkinWearId != SkinItemConfig.Id) {
            _delayedTrigger.DelayedTriggerAction(WearNewSkin);
        }
    }
    
    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        
        _delayedTrigger.CancelTriggerAction();
    }

    private void BuyNewSkin() {
        _saver.GetSave.AddNewSkin(SkinItemConfig.Id);
        _moneyIcon.DisactiveSelf();
        WearNewSkin();
        _playerBank.GiveMeYourFuckingMoneyNigga(SkinItemConfig.Price);
    }

    private void WearNewSkin() {

        Debug.Log("Надеваание скина " + _localization.GetSkinName(SkinItemConfig.Id));
        _saver.GetSave.SkinWearId = SkinItemConfig.Id;
        _playerSkinWear.WearNewSkin(SkinItemConfig);
        _priceText.text = _localization.IsWeared;
        _saver.Save();
    }

    
}
