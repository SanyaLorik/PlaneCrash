using Architecture_M;
using Cysharp.Threading.Tasks.Triggers;
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
    [Inject] protected IGameSave<GameSavePC> _saver; 
    [Inject] protected LocalizationDataPC _localization; 
    [Inject] protected PlayerBank _playerBank;
    [Inject] protected PlayerSkinWear _playerSkinWear;
    [Inject] protected RectTransformHelper _fillAmounthMover;
    
    
    
    public void InitSkinData() {
        _playerSkinWear.NewSkinWear += CheckSelect;
        _wearText.text = _localization.IsWeared;
        _nameText.text = _localization.GetSkinName(SkinItemConfig.Id);
        
        if (_saver.GetSave.SkinIsBought(SkinItemConfig.Id)) {
            HideGetVisual();
            
            if(_saver.GetSave.SkinWearId == SkinItemConfig.Id) {
                WearNewSkin();
            }
        }
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
    
    protected bool SkinIsWeared() 
        => _saver.GetSave.SkinWearId == SkinItemConfig.Id;
    

    protected bool SkinIsBought() 
        => _saver.GetSave.SkinIsBought(SkinItemConfig.Id);
    
    
    
    private void CheckSelect() {
        if (_saver.GetSave.SkinIsBought(SkinItemConfig.Id) && _saver.GetSave.SkinWearId != SkinItemConfig.Id) {
            _wearText.DisactiveSelf();
        }
    }
    
    
    protected void WearNewSkin() {
        Debug.Log("Надеваание скина " + _localization.GetSkinName(SkinItemConfig.Id));
        _saver.GetSave.SkinWearId = SkinItemConfig.Id;
        _playerSkinWear.WearNewSkin(SkinItemConfig);
        _wearText.ActiveSelf();
        HideGetVisual();
        _saver.Save();
    }

    protected void HideGetVisual() {
        _priceContainer.DisactiveSelf();
        _advContainer.DisactiveSelf();
    }
    
    private void OnTriggerExit(Collider collider) {
        if(!collider.TryGetComponent(out PlayerMovement _)) return;
        _delayedTrigger.CancelTriggerAction();
    }
    
    protected abstract void InitSpecific();
    protected abstract void GetNewSkin();

}